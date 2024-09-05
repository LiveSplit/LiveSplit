using System;
using System.Drawing;
using System.Linq;
using System.Net;

using LiveSplit.Model;
using LiveSplit.Options;

using SpeedrunComSharp;

namespace LiveSplit.Web.Share;

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
        {
            return true;
        }

        string accessToken = Authenticator?.GetAccessToken();
        if (string.IsNullOrEmpty(accessToken))
        {
            return false;
        }

        Client.AccessToken = accessToken;

        bool isTokenValid = Client.IsAccessTokenValid;

        if (isTokenValid)
        {
            try
            {
                WebCredentials.SpeedrunComAccessToken = accessToken;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        return isTokenValid;
    }

    public static Time ToTime(this RunTimes times)
    {
        var time = new Time(realTime: times.RealTime);

        if (times.GameTime.HasValue)
        {
            time.GameTime = times.GameTime.Value;
        }
        else if (times.RealTimeWithoutLoads.HasValue)
        {
            time.GameTime = times.RealTimeWithoutLoads.Value;
        }

        return time;
    }
    public static IRun GetRun(this SpeedrunComSharp.Run record)
    {
        string apiUri = record.SplitsUri.AbsoluteUri;
        string path = apiUri[(apiUri.LastIndexOf("/") + 1)..];
        return SplitsIO.Instance.DownloadRunByPath(path, false);
    }

    public static Model.TimingMethod ToLiveSplitTimingMethod(this SpeedrunComSharp.TimingMethod timingMethod)
    {
        return timingMethod switch
        {
            SpeedrunComSharp.TimingMethod.RealTime => Model.TimingMethod.RealTime,
            SpeedrunComSharp.TimingMethod.GameTime or SpeedrunComSharp.TimingMethod.RealTimeWithoutLoads => Model.TimingMethod.GameTime,
            _ => throw new ArgumentException("timingMethod"),
        };
    }

    public static Image GetBoxartImage(this Assets assets)
    {
        var request = WebRequest.Create(assets.CoverMedium.Uri);

        using WebResponse response = request.GetResponse();
        using System.IO.Stream stream = response.GetResponseStream();
        return Image.FromStream(stream);
    }

    public static Image GetIconImage(this Assets assets)
    {
        var request = WebRequest.Create(assets.Icon.Uri);

        using WebResponse response = request.GetResponse();
        using System.IO.Stream stream = response.GetResponseStream();
        return Image.FromStream(stream);
    }

    public static void PatchRun(this IRun run, SpeedrunComSharp.Run srdcRun)
    {
        run.GameName = srdcRun.Game.Name;
        run.CategoryName = srdcRun.Category.Name;
        run.Metadata.PlatformName = srdcRun.System.Platform.Name;
        run.Metadata.RegionName = srdcRun.System.Region?.Name;
        run.Metadata.UsesEmulator = srdcRun.System.IsEmulated;
        run.Metadata.VariableValueNames = srdcRun.VariableValues.ToDictionary(x => x.Name, x => x.Value);
        run.Metadata.RunID = srdcRun.ID;
    }

    public static DateTime? FindPersonalBestAttemptDate(IRun run)
    {
        Time runTime = run.Last().PersonalBestSplitTime;

        Attempt attempt = run.AttemptHistory.FirstOrDefault(x =>
            x.Time.GameTime == runTime.GameTime
            && x.Time.RealTime == runTime.RealTime);

        return attempt.Ended?.Time;
    }

    public static bool ValidateRun(IRun run, out string reasonForRejection)
    {
        try
        {
            RunMetadata metadata = run.Metadata;

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

            if (metadata.Category.Players.Value > 1 && metadata.Category.Players.Type == PlayersType.Exactly)
            {
                reasonForRejection = "Submitting runs for more than the currently authenticated user is not implemented yet.";
                return false;
            }

            if (metadata.Platform == null)
            {
                reasonForRejection = "You need to specify the platform of the game.";
                return false;
            }

            Model.TimingMethod primaryTimingMethod = metadata.Game.Ruleset.DefaultTimingMethod.ToLiveSplitTimingMethod();

            Time runTime = run.Last().PersonalBestSplitTime;

            if (!runTime.RealTime.HasValue)
            {
                reasonForRejection = "You can't submit a run without a Real Time.";
                return false;
            }

            System.Collections.Generic.KeyValuePair<Variable, VariableValue> variableThatNeedsAValueButHasNone = metadata.VariableValues.Where(x => x.Key.IsMandatory).FirstOrDefault(x => x.Value == null);
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

    public static void ClearAccessToken()
    {
        Client.AccessToken = null;
    }

    public static bool SubmitRun(IRun run, out string reasonForRejection,
        string comment = null, Uri videoUri = null, DateTime? date = null,
        TimeSpan? withoutLoads = null,
        bool submitToSplitsIO = true)
    {
        try
        {
            RunMetadata metadata = run.Metadata;

            bool isValid = ValidateRun(run, out reasonForRejection);
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

            System.Collections.ObjectModel.ReadOnlyCollection<SpeedrunComSharp.TimingMethod> timingMethods = metadata.Game.Ruleset.TimingMethods;
            Time runTime = run.Last().PersonalBestSplitTime;

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
                string categoryId = metadata.Category.ID;
                string platformId = metadata.Platform.ID;
                string regionId = metadata.Region?.ID;
                TimeSpan? realTime = timingMethods.Contains(SpeedrunComSharp.TimingMethod.RealTime) ? runTime.RealTime : null;
                TimeSpan? realTimeWithoutLoads = timingMethods.Contains(SpeedrunComSharp.TimingMethod.RealTimeWithoutLoads) ? runTime.GameTime : null;

                if (withoutLoads.HasValue)
                {
                    realTimeWithoutLoads = withoutLoads;
                }

                TimeSpan? gameTime = timingMethods.Contains(SpeedrunComSharp.TimingMethod.GameTime) ? runTime.GameTime : null;

                bool emulated = metadata.Game.Ruleset.EmulatorsAllowed && metadata.UsesEmulator;
                System.Collections.Generic.IEnumerable<VariableValue> variables = metadata.VariableValues.Values.Where(x => x != null);

                Uri splitsIOUri = null;
                string splitsIORunId = null;
                string claimToken = null;

                if (submitToSplitsIO)
                {
                    try
                    {
                        var uri = new Uri(SplitsIO.Instance.Share(run, claimTokenUri: true));
                        splitsIOUri = SplitsIO.Instance.GetSiteUri(uri.AbsolutePath);

                        string[] splitted = uri.Query.Split('?', '=', '&');
                        claimToken = splitted.SkipWhile(x => x != "claim_token").Skip(1).FirstOrDefault();
                        splitsIORunId = uri.AbsolutePath[1..];
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }
                }

                SpeedrunComSharp.Run submittedRun = Client.Runs.Submit(
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
