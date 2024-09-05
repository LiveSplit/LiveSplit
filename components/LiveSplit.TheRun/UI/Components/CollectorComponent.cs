using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Xml;

using LiveSplit.Model;

namespace LiveSplit.UI.Components;

public class CollectorComponent : LogicComponent
{
    public override string ComponentName => "therun.gg";

    private LiveSplitState State { get; set; }
    private CollectorSettings Settings { get; set; }

    private readonly HttpClient httpClient;

    private string SplitWebhookUrl => "https://dspc6ekj2gjkfp44cjaffhjeue0fbswr.lambda-url.eu-west-1.on.aws/";
    private string FileUploadBaseUrl => "https://2uxp372ks6nwrjnk6t7lqov4zu0solno.lambda-url.eu-west-1.on.aws/";

    private string GameName = "";
    private string CategoryName = "";

    private bool TimerPaused = false;
    private bool WasJustResumed = false;
    private TimeSpan CurrentPausedTime = TimeSpan.Zero;
    private TimeSpan TimePausedBeforeResume = TimeSpan.Zero;

    public CollectorComponent(LiveSplitState state)
    {
        State = state;
        Settings = new CollectorSettings();

        httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
        httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Site", "cross-site");
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Disposition", "attachment");

        SetGameAndCategory();

        State.OnStart += HandleSplit;
        State.OnSplit += HandleSplit;
        State.OnSkipSplit += HandleSplit;
        State.OnUndoSplit += HandleSplit;
        State.OnUndoAllPauses += HandleSplit;

        State.OnPause += HandlePause;
        State.OnResume += HandleResume;
        State.OnReset += HandleReset;
    }

    public async Task UpdateSplitsState()
    {
        object returnData = buildLiveRunData();

        var serializer = new JavaScriptSerializer();
        var content = new StringContent(serializer.Serialize(returnData));

        await httpClient.PostAsync(SplitWebhookUrl, content);
    }

    private void SetGameAndCategory()
    {
        GameName = State.Run.GameName;
        CategoryName = State.Run.CategoryName;
    }

    private object buildLiveRunData()
    {
        IRun run = State.Run;
        TimeSpan? currentTime = State.CurrentTime[State.CurrentTimingMethod];
        List<object> runData = [];

        var metaData = new
        {
            game = GameName,
            category = CategoryName,
            platform = run.Metadata.PlatformName,
            region = run.Metadata.RegionName,
            emulator = run.Metadata.UsesEmulator,
            variables = run.Metadata.VariableValueNames
        };

        foreach (ISegment segment in run)
        {
            List<object> comparisons = [];

            foreach (string key in segment.Comparisons.Keys)
            {
                comparisons.Add(new
                {
                    name = key,
                    time = ConvertTime(segment.Comparisons[key])
                });
            }

            runData.Add(new
            {
                name = segment.Name,
                splitTime = ConvertTime(segment.SplitTime),
                pbSplitTime = ConvertTime(segment.PersonalBestSplitTime),
                bestPossible = ConvertTime(segment.BestSegmentTime),
                comparisons = comparisons
            });
        }

        return new
        {
            metadata = metaData,
            currentTime = ConvertTime(State.CurrentTime),
            currentSplitName = State.CurrentSplit != null ? State.CurrentSplit.Name : "",
            currentSplitIndex = State.CurrentSplitIndex,
            timingMethod = State.CurrentTimingMethod,
            currentDuration = State.CurrentAttemptDuration.TotalMilliseconds,
            startTime = State.AttemptStarted.Time.ToUniversalTime(),
            endTime = State.AttemptEnded.Time.ToUniversalTime(),
            uploadKey = Settings.UploadKey,
            isPaused = TimerPaused,
            isGameTimePaused = State.IsGameTimePaused,
            gameTimePauseTime = State.GameTimePauseTime,
            totalPauseTime = State.PauseTime,
            currentPauseTime = TimePausedBeforeResume,
            timePausedAt = State.TimePausedAt.TotalMilliseconds,
            wasJustResumed = WasJustResumed,
            currentComparison = State.CurrentComparison,
            runData = runData
        };
    }

    private double? ConvertTime(Time time)
    {
        if (time[State.CurrentTimingMethod] == null)
        {
            return null;
        }

        var timeSpan = (TimeSpan)time[State.CurrentTimingMethod];

        return timeSpan.TotalMilliseconds;
    }

#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously.
    public async void HandlePause(object sender, object e)
#pragma warning restore CS1998
    {
        TimerPaused = true;
        HandleSplit(sender, e);
    }

#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously.
    public async void HandleResume(object sender, object e)
#pragma warning restore CS1998
    {

        TimePausedBeforeResume = (TimeSpan)(State.PauseTime - CurrentPausedTime);
        CurrentPausedTime = (TimeSpan)State.PauseTime;
        TimerPaused = false;
        WasJustResumed = true;

        HandleSplit(sender, e);
    }

    // TODO: Log or tell user when splits are invalid or when an error occurs. Don't just continue silently.
    public async void HandleSplit(object sender, object e)
    {
        SetGameAndCategory();
        if (!AreSplitsValid() || !Settings.IsLiveTrackingEnabled)
        {
            return;
        }

        try
        {
            await UpdateSplitsState();

            if (State.CurrentSplitIndex == State.Run.Count)
            {
                await UploadSplits();
            }
        }
        catch { }

        WasJustResumed = false;
    }

    public async void HandleReset(object sender, TimerPhase value)
    {
        SetGameAndCategory();
        if (!AreSplitsValid())
        {
            return;
        }

        try
        {
            if (Settings.IsLiveTrackingEnabled)
            {
                await UpdateSplitsState();
            }

            await UploadSplits();
        }
        catch { }
    }

    private bool AreSplitsValid()
    {
        return GameName != "" && CategoryName != "" && Settings.UploadKey.Length == 36;
    }

    public async Task UploadSplits()
    {
        if (!Settings.IsStatsUploadingEnabled)
        {
            return;
        }

        string fileName = HttpUtility.UrlEncode(GameName) + "-" + HttpUtility.UrlEncode(CategoryName) + ".lss";
        string fileUploadUrl = FileUploadBaseUrl + "?filename=" + fileName + "&uploadKey=" + Settings.UploadKey;

        HttpResponseMessage result = await httpClient.GetAsync(fileUploadUrl);
        string responseBody = await result.Content.ReadAsStringAsync();

        // Something went wrong, but the backend will handle the error, LiveSplit should just keep going.
        // Probably the upload key was not filled in.
        if (!result.IsSuccessStatusCode)
        {
            return;
        }

        var ser = new JavaScriptSerializer();
        Dictionary<string, string> jsonObj = ser.Deserialize<Dictionary<string, string>>(responseBody);

        string url = HttpUtility.UrlDecode(jsonObj["url"]);
        string correctlyEncodedUrl = EncodeUrl(url);

        var content = new StringContent(XmlRunAsString());
        content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");

        await httpClient.PutAsync(correctlyEncodedUrl, content);
    }

    private string EncodeUrl(string url)
    {
        string[] urlParts = url.Split('&').Select(urlPart => urlPart.StartsWith("X-Amz-Credential") || urlPart.StartsWith("X-Amz-Security-Token") || urlPart.StartsWith("X-Amz-SignedHeaders") ? HttpUtility.UrlEncode(urlPart).Replace("%3d", "=") : urlPart).ToArray();

        string newUrl = string.Join("&", urlParts).Replace(GameName, HttpUtility.UrlEncode(GameName)).Replace(CategoryName, HttpUtility.UrlEncode(CategoryName));
        string username = newUrl.Replace("https://splits-bucket-main.s3.eu-west-1.amazonaws.com/", "").Split('/')[0];

        return newUrl.Replace(username, HttpUtility.UrlEncode(username));
    }

    private string XmlRunAsString()
    {
        var runSaver = new Model.RunSavers.XMLRunSaver();
        var stream = new System.IO.MemoryStream();

        runSaver.Save(State.Run, stream);

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    public override void Dispose()
    {
        State.OnStart -= HandleSplit;
        State.OnSplit -= HandleSplit;
        State.OnSkipSplit -= HandleSplit;
        State.OnUndoSplit -= HandleSplit;
        State.OnReset -= HandleReset;

        httpClient.Dispose();
    }

    public override XmlNode GetSettings(XmlDocument document)
    {
        return Settings.GetSettings(document);
    }

    public override Control GetSettingsControl(LayoutMode mode)
    {
        Settings.Mode = mode;
        return Settings;
    }

    public override void SetSettings(XmlNode settings)
    {
        Settings.SetSettings(settings);
    }

    public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode) { }
}
