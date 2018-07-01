using LiveSplit.Model;
using LiveSplit.Options;
using SpeedrunComSharp;
using System;
using System.Drawing;
using System.Linq;
using System.Net;

namespace LiveSplit.Web.Share
{
    public static class SpeedrunCom
    {
        public static SpeedrunComClient Client { get; private set; }

        public static ISpeedrunComAuthenticator Authenticator { private get; set; }

        static SpeedrunCom()
        {
            Client = new SpeedrunComClient(Updates.UpdateHelper.UserAgent, WebCredentials.SpeedrunComAccessToken);
        }

        public static bool MakeSureUserIsAuthenticated()
        {
            if (Client.IsAccessTokenValid)
                return true;

            var accessToken = Authenticator?.GetAccessToken();
            if (string.IsNullOrEmpty(accessToken))
                return false;

            Client.AccessToken = accessToken;

            return Client.IsAccessTokenValid;
        }

        public static Time ToTime(this RunTimes times)
        {
            var time = new Time(realTime: times.RealTime);

            if (times.GameTime.HasValue)
                time.GameTime = times.GameTime.Value;
            else if (times.RealTimeWithoutLoads.HasValue)
                time.GameTime = times.RealTimeWithoutLoads.Value;

            return time;
        }
        public static IRun GetRun(this SpeedrunComSharp.Run record)
        {
            var apiUri = record.SplitsUri.AbsoluteUri;
            var path = apiUri.Substring(apiUri.LastIndexOf("/") + 1);
            return SplitsIO.Instance.DownloadRunByPath(path, false);
        }

        public static Model.TimingMethod ToLiveSplitTimingMethod(this SpeedrunComSharp.TimingMethod timingMethod)
        {
            switch (timingMethod)
            {
                case SpeedrunComSharp.TimingMethod.RealTime:
                    return Model.TimingMethod.RealTime;
                case SpeedrunComSharp.TimingMethod.GameTime:
                case SpeedrunComSharp.TimingMethod.RealTimeWithoutLoads:
                    return Model.TimingMethod.GameTime;
            }

            throw new ArgumentException("timingMethod");
        }

        public static Image GetBoxartImage(this Assets assets)
        {
            var request = WebRequest.Create(assets.CoverMedium.Uri);

            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                return Image.FromStream(stream);
            }
        }

        public static Image GetIconImage(this Assets assets)
        {
            var request = WebRequest.Create(assets.Icon.Uri);

            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                return Image.FromStream(stream);
            }
        }

        public static void PatchRun(this IRun run, SpeedrunComSharp.Run srdcRun)
        {
            run.GameName = srdcRun.Game.Name;
            run.CategoryName = srdcRun.Category.Name;
            run.Metadata.PlatformName = srdcRun.System.Platform.Name;
            run.Metadata.RegionName = srdcRun.System.Region != null ? srdcRun.System.Region.Name : null;
            run.Metadata.UsesEmulator = srdcRun.System.IsEmulated;
            run.Metadata.VariableValueNames = srdcRun.VariableValues.ToDictionary(x => x.Name, x => x.Value);
            run.Metadata.RunID = srdcRun.ID;
        }

        public static DateTime? FindPersonalBestAttemptDate(IRun run)
        {
            var runTime = run.Last().PersonalBestSplitTime;

            var attempt = run.AttemptHistory.FirstOrDefault(x => 
                x.Time.GameTime == runTime.GameTime 
                && x.Time.RealTime == runTime.RealTime);

            return attempt.Ended?.Time;
        }

        public static bool ValidateRun(IRun run, out string reasonForRejection)
        {
            try
            {
                var metadata = run.Metadata;

                if (!string.IsNullOrEmpty(metadata.RunID))
                {
                    reasonForRejection = "This run already exists on speedrun.com.";
                    return false;
                }

                if (!MakeSureUserIsAuthenticated())
                {
                    reasonForRejection = "You can't submit a run without being authenticated.";
                    return false;
                }

                if (string.IsNullOrEmpty(run.GameName))
                {
                    reasonForRejection = "You need to specify a game.";
                    return false;
                }

                if (metadata.Game == null)
                {
                    reasonForRejection = "The game could not be found on speedrun.com.";
                    return false;
                }

                if (string.IsNullOrEmpty(run.CategoryName))
                {
                    reasonForRejection = "You need to specify a category.";
                    return false;
                }

                if (metadata.Category == null)
                {
                    reasonForRejection = "The category could not be found on speedrun.com.";
                    return false;
                }

                if (metadata.Category.Players.Value > 1)
                {
                    reasonForRejection = "Submitting runs for more than the currently authenticated user is not implemented yet.";
                    return false;
                }

                if (metadata.Platform == null)
                {
                    reasonForRejection = "You need to specify the platform of the game.";
                    return false;
                }

                var primaryTimingMethod = metadata.Game.Ruleset.DefaultTimingMethod.ToLiveSplitTimingMethod();

                var runTime = run.Last().PersonalBestSplitTime;

                if (!runTime.RealTime.HasValue)
                {
                    reasonForRejection = "You can't submit a run without a Real Time.";
                    return false;
                }

                var variableThatNeedsAValueButHasNone = metadata.VariableValues.Where(x => x.Key.IsMandatory).FirstOrDefault(x => x.Value == null);
                if (variableThatNeedsAValueButHasNone.Value != null)
                {
                    reasonForRejection = $"You need to specify a value for the variable \"{variableThatNeedsAValueButHasNone.Key.Name}\".";
                    return false;
                }

                reasonForRejection = null;
                return true;
            }
            catch (Exception ex)
            {
                reasonForRejection = "The run could not be validated for an unknown reason.";
                Log.Error(ex);
                return false;
            }
        }

        public static bool SubmitRun(IRun run, out string reasonForRejection, 
            string comment = null, Uri videoUri = null, DateTime? date = null,
            TimeSpan? withoutLoads = null,
            bool submitToSplitsIO = true)
        {
            try
            {
                var metadata = run.Metadata;

                var isValid = ValidateRun(run, out reasonForRejection);
                if (!isValid)
                {
                    return false;
                }

                //This is legal for mods, so we either have to check the mods or rely on the API responding
                //This results in unnecessary submits to splits i/o though.
                //This is also ignoring series moderators. That's such a rare case that it probably never happens.
                if (metadata.Game.Ruleset.RequiresVideo && videoUri == null && !metadata.Game.ModeratorUsers.Contains(Client.Profile))
                {
                    reasonForRejection = "Runs of this game require a video.";
                    return false;
                }

                var timingMethods = metadata.Game.Ruleset.TimingMethods;
                var runTime = run.Last().PersonalBestSplitTime;

                if (date == null)
                {
                    date = FindPersonalBestAttemptDate(run);
                }

                if (date.HasValue && date.Value.ToUniversalTime().Date > DateTime.UtcNow.Date)
                {
                    reasonForRejection = "The date of the run can't be in the future.";
                    return false;
                }

                if (date.HasValue && metadata.Game.YearOfRelease.HasValue && date.Value.ToUniversalTime().Date.Year < metadata.Game.YearOfRelease)
                {
                    reasonForRejection = "The date of the run can't be before the release date of the game.";
                    return false;
                }

                try
                {
                    var categoryId = metadata.Category.ID;
                    var platformId = metadata.Platform.ID;
                    var regionId = metadata.Region != null ? metadata.Region.ID : null;
                    var realTime = timingMethods.Contains(SpeedrunComSharp.TimingMethod.RealTime) ? runTime.RealTime : null;
                    var realTimeWithoutLoads = timingMethods.Contains(SpeedrunComSharp.TimingMethod.RealTimeWithoutLoads) ? runTime.GameTime : null;

                    if (withoutLoads.HasValue)
                    {
                        realTimeWithoutLoads = withoutLoads;
                    }

                    var gameTime = timingMethods.Contains(SpeedrunComSharp.TimingMethod.GameTime) ? runTime.GameTime : null;

                    var emulated = metadata.Game.Ruleset.EmulatorsAllowed && metadata.UsesEmulator;
                    var variables = metadata.VariableValues.Values.Where(x => x != null);

                    Uri splitsIOUri = null;
                    string splitsIORunId = null;
                    string claimToken = null;

                    if (submitToSplitsIO)
                    {
                        try
                        {
                            var uri = new Uri(SplitsIO.Instance.Share(run, claimTokenUri: true));
                            splitsIOUri = SplitsIO.Instance.GetSiteUri(uri.AbsolutePath);

                            var splitted = uri.Query.Split('?', '=', '&');
                            claimToken = splitted.SkipWhile(x => x != "claim_token").Skip(1).FirstOrDefault();
                            splitsIORunId = uri.AbsolutePath.Substring(1);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex);
                        }
                    }

                    var submittedRun = Client.Runs.Submit(
                        //simulateSubmitting: true,
                        categoryId: categoryId,
                        platformId: platformId,
                        regionId: regionId,
                        realTime: realTime,
                        realTimeWithoutLoads: realTimeWithoutLoads,
                        gameTime: gameTime,
                        comment: comment,
                        videoUri: videoUri,
                        date: date,
                        emulated: emulated,
                        splitsIOUri: splitsIOUri,
                        verify: false,
                        variables: variables
                        );

                    if (submitToSplitsIO
                        && !string.IsNullOrEmpty(submittedRun.ID)
                        && !string.IsNullOrEmpty(splitsIORunId)
                        && !string.IsNullOrEmpty(claimToken))
                    {
                        try
                        {
                            SplitsIO.Instance.ClaimWithSpeedrunComRun(splitsIORunId, claimToken, submittedRun.ID);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex);
                        }
                    }

                    run.Metadata.Run = submittedRun;
                }
                catch (APIException ex)
                {
                    reasonForRejection = string.Join(Environment.NewLine, ex.Errors);
                    return false;
                }

                reasonForRejection = null;
                return true;
            }
            catch (Exception ex)
            {
                reasonForRejection = "The run could not be submitted for an unknown reason.";
                Log.Error(ex);
                return false;
            }
        }
    }
}
