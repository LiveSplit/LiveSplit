using LiveSplit.Model;
using LiveSplit.Model.Comparisons;
using LiveSplit.Options;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using LiveSplit.View;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Xunit;

namespace LiveSplit.Tests.AutoSplitterTests;

[CollectionDefinition("AutoSplitter startup", DisableParallelization = true)]
public class AutoSplitterStartupCollection;

[Collection("AutoSplitter startup")]
public class AutoSplitterStartupTests
{
    [Fact]
    public void PublishListBeforeNotifyingConsumers()
    {
        var factory = (AutoSplitterFactory)Activator.CreateInstance(typeof(AutoSplitterFactory), true);
        var splitter = new AutoSplitter();
        var splitters = new Dictionary<string, AutoSplitter> { ["test game"] = splitter };
        bool observed = false;
        EventHandler handler = (_, _) => observed = ReferenceEquals(splitters, factory.AutoSplitters)
            && ReferenceEquals(splitter, factory.Create("Test Game"));
        factory.AutoSplittersChanged += handler;

        factory.AutoSplitters = splitters;

        Assert.True(observed, "Consumers must be notified after the new list is available.");
    }

    [Fact]
    public void PopulateOpenEditorWhenListArrivesWithoutChangingTheGame()
    {
        OnStaThread(() =>
        {
            AutoSplitterFactory original = AutoSplitterFactory.Instance;
            var factory = (AutoSplitterFactory)Activator.CreateInstance(typeof(AutoSplitterFactory), true);
            typeof(AutoSplitterFactory).GetProperty(nameof(AutoSplitterFactory.Instance)).SetValue(null, factory);
            try
            {
                factory.AutoSplitters = null;
                var run = new Run(new StandardComparisonGeneratorsFactory()) { GameName = "Test Game" };
                run.AddSegment("Finish");
                var state = new LiveSplitState(run, null, null, null, new Settings());
                using var editor = new RunEditorDialog(state);
                state.Form = editor;
                _ = editor.Handle;
                Application.DoEvents();
                Assert.Null(run.AutoSplitter);

                var splitter = new AutoSplitter { Description = "Test description", Games = ["test game"], URLs = ["https://example.invalid/test.asl"] };
                Task.Run(() => factory.AutoSplitters = new Dictionary<string, AutoSplitter> { ["test game"] = splitter }).GetAwaiter().GetResult();
                PumpUntil(() => run.AutoSplitter != null);

                Assert.Same(splitter, run.AutoSplitter);
                var description = (Label)typeof(RunEditorDialog).GetField("lblDescription", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(editor);
                Assert.Equal("Test description", description.Text);
                Assert.False(splitter.IsActivated);
            }
            finally
            {
                typeof(AutoSplitterFactory).GetProperty(nameof(AutoSplitterFactory.Instance)).SetValue(null, original);
            }
        });
    }

    [Theory]
    [InlineData("unchanged")]
    [InlineData("game changed")]
    [InlineData("run replaced")]
    [InlineData("disabled")]
    [InlineData("deactivated")]
    [InlineData("started")]
    [InlineData("closed")]
    [InlineData("download failed with cache")]
    [InlineData("download failed without cache")]
    [InlineData("timed out")]
    public void RecheckCurrentRunAfterDownloading(string change)
    {
        OnStaThread(() =>
        {
            string originalPath = ComponentManager.BasePath;
            var originalFactories = ComponentManager.ComponentFactories;
            string directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(Path.Combine(directory, "Components"));
            using var server = new DelayedServer(change.StartsWith("download failed") ? 503 : 200);
            using var form = new Form();
            _ = form.Handle;
            try
            {
                ComponentManager.BasePath = directory;
                var componentFactory = new ScriptFactory();
                typeof(ComponentManager).GetProperty(nameof(ComponentManager.ComponentFactories)).SetValue(null,
                    new Dictionary<string, IComponentFactory> { ["LiveSplit.ScriptableAutoSplit.dll"] = componentFactory });
                var splitter = new AutoSplitter { Type = AutoSplitterType.Script, Games = ["test game"], URLs = [server.Url] };
                if (change == "download failed with cache")
                {
                    File.WriteAllText(splitter.LocalPath, "cached script");
                }
                var run = new Run(new StandardComparisonGeneratorsFactory()) { GameName = "Test Game", AutoSplitter = splitter };
                var settings = new XmlDocument();
                settings.LoadXml("<Settings gameName='Test Game'><Value>42</Value></Settings>");
                run.AutoSplitterSettings = settings.DocumentElement;
                var state = new LiveSplitState(run, form, null, null, new Settings { ActiveAutoSplitters = ["Test Game"] });

                Task activation = splitter.ActivateAsync(state);
                Assert.False(activation.IsCompleted, "Activation must yield while the network is pending.");
                Task secondActivation = splitter.ActivateAsync(state);
                PumpUntil(() => server.Requested.Task.IsCompleted);
                switch (change)
                {
                    case "game changed": run.GameName = "Other Game"; break;
                    case "run replaced": state.Run = new Run(new StandardComparisonGeneratorsFactory()) { GameName = "Test Game", AutoSplitter = splitter }; break;
                    case "disabled": state.Settings.ActiveAutoSplitters.Clear(); break;
                    case "deactivated": splitter.Deactivate(); break;
                    case "started": state.CurrentPhase = TimerPhase.Running; break;
                    case "closed": form.Dispose(); break;
                }
                if (change != "timed out")
                {
                    server.Release.TrySetResult(true);
                }
                PumpUntil(() => activation.IsCompleted && secondActivation.IsCompleted);
                activation.GetAwaiter().GetResult();
                secondActivation.GetAwaiter().GetResult();

                bool missing = change == "download failed without cache" || change == "timed out";
                Assert.Equal(!missing, File.Exists(splitter.LocalPath));
                if (!missing)
                {
                    Assert.Equal(change == "download failed with cache" ? "cached script" : "test script", File.ReadAllText(splitter.LocalPath));
                }
                Assert.Empty(Directory.GetFiles(Path.Combine(directory, "Components"), "*.tmp"));
                Assert.Equal(change == "unchanged" || change == "download failed with cache", splitter.IsActivated);
                if (change == "unchanged")
                {
                    Assert.Same(run.AutoSplitterSettings, componentFactory.Created.Settings);
                    Assert.Equal(Thread.CurrentThread.ManagedThreadId, componentFactory.CreationThread);
                    splitter.ActivateAsync(state).GetAwaiter().GetResult();
                    Assert.Equal(1, componentFactory.CreationCount);
                }
                else if (change == "started" || change == "deactivated")
                {
                    state.CurrentPhase = TimerPhase.NotRunning;
                    splitter.ActivateAsync(state).GetAwaiter().GetResult();
                    Assert.True(splitter.IsActivated);
                }
                else if (change == "download failed without cache")
                {
                    using var retryServer = new DelayedServer();
                    splitter.URLs = [retryServer.Url];
                    Task retry = splitter.ActivateAsync(state);
                    PumpUntil(() => retryServer.Requested.Task.IsCompleted);
                    retryServer.Release.TrySetResult(true);
                    PumpUntil(() => retry.IsCompleted);
                    retry.GetAwaiter().GetResult();
                    Assert.True(splitter.IsActivated);
                }
                splitter.Deactivate();
            }
            finally
            {
                ComponentManager.BasePath = originalPath;
                typeof(ComponentManager).GetProperty(nameof(ComponentManager.ComponentFactories)).SetValue(null, originalFactories);
                Directory.Delete(directory, true);
            }
        });
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void SelectTheEditedGameBeforeStartingItsDownload(bool gameExists)
    {
        OnStaThread(() =>
        {
            AutoSplitterFactory original = AutoSplitterFactory.Instance;
            var factory = (AutoSplitterFactory)Activator.CreateInstance(typeof(AutoSplitterFactory), true);
            typeof(AutoSplitterFactory).GetProperty(nameof(AutoSplitterFactory.Instance)).SetValue(null, factory);
            using var server = new DelayedServer();
            var previous = new AutoSplitter { Games = ["old game"], URLs = [server.Url], Description = "Old", Component = new RecordingComponent() };
            var next = new AutoSplitter { Type = AutoSplitterType.Script, Games = ["new game"], URLs = [server.Url], Description = "New" };
            factory.AutoSplitters = new Dictionary<string, AutoSplitter> { ["old game"] = previous };
            if (gameExists) factory.AutoSplitters.Add("new game", next);
            var run = new Run(new StandardComparisonGeneratorsFactory()) { GameName = "Old Game", AutoSplitter = previous };
            run.AddSegment("Finish");
            var state = new LiveSplitState(run, null, null, null, new Settings { ActiveAutoSplitters = ["New Game"] });
            try
            {
                using var editor = new RunEditorDialog(state);
                state.Form = editor;
                _ = editor.Handle;
                Application.DoEvents();
                var combo = Field<ComboBox>(editor, "cbxGameName");
                _ = combo.Handle;
                combo.Text = "Old Game";
                Assert.True(Field<Button>(editor, "btnSettings").Enabled);
                typeof(RunEditorDialog).GetField("IsInitialized", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(editor, true);
                combo.Text = gameExists ? "New Game" : "Missing Game";

                Assert.Equal("Old Game", run.GameName);
                Assert.Equal(gameExists, run.AutoSplitter != null);
                Assert.False(Field<Button>(editor, "btnSettings").Enabled);
                if (gameExists)
                {
                    typeof(RunEditorDialog).GetMethod("cbxGameName_Validated", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(editor, [combo, EventArgs.Empty]);
                    PumpUntil(() => server.Requested.Task.IsCompleted);
                    Assert.Equal("New Game", run.GameName);
                }
            }
            finally
            {
                typeof(AutoSplitterFactory).GetProperty(nameof(AutoSplitterFactory.Instance)).SetValue(null, original);
            }
        });
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void ActivateOrCancelThroughTheEditorButtonWithoutBlocking(bool running, bool cancel)
    {
        OnStaThread(() =>
        {
            string directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(Path.Combine(directory, "Components"));
            string originalPath = ComponentManager.BasePath;
            var originalFactories = ComponentManager.ComponentFactories;
            AutoSplitterFactory original = AutoSplitterFactory.Instance;
            var factory = (AutoSplitterFactory)Activator.CreateInstance(typeof(AutoSplitterFactory), true);
            typeof(AutoSplitterFactory).GetProperty(nameof(AutoSplitterFactory.Instance)).SetValue(null, factory);
            using var server = new DelayedServer();
            try
            {
                ComponentManager.BasePath = directory;
                typeof(ComponentManager).GetProperty(nameof(ComponentManager.ComponentFactories)).SetValue(null,
                    new Dictionary<string, IComponentFactory> { ["LiveSplit.ScriptableAutoSplit.dll"] = new ScriptFactory() });
                var splitter = new AutoSplitter { Type = AutoSplitterType.Script, Games = ["test game"], URLs = [server.Url], Description = "Test" };
                factory.AutoSplitters = new Dictionary<string, AutoSplitter> { ["test game"] = splitter };
                File.WriteAllText(splitter.LocalPath, "cached script");
                var run = new Run(new StandardComparisonGeneratorsFactory()) { GameName = "Test Game", AutoSplitter = splitter };
                run.AddSegment("Finish");
                var state = new LiveSplitState(run, null, null, null, new Settings());
                if (running) state.CurrentPhase = TimerPhase.Running;
                using var editor = new RunEditorDialog(state);
                state.Form = editor;
                _ = editor.Handle;
                Application.DoEvents();
                var click = typeof(RunEditorDialog).GetMethod("btnActivate_Click", BindingFlags.Instance | BindingFlags.NonPublic);
                var watch = Stopwatch.StartNew();
                click.Invoke(editor, [editor, EventArgs.Empty]);
                watch.Stop();
                PumpUntil(() => server.Requested.Task.IsCompleted);
                factory.AutoSplitters = new Dictionary<string, AutoSplitter> { ["test game"] = splitter };
                Application.DoEvents();
                Task activation = splitter.ActivateAsync(state, allowWhileRunning: true);
                Assert.Equal("Deactivate", Field<Button>(editor, "btnActivate").Text);
                if (cancel) click.Invoke(editor, [editor, EventArgs.Empty]);
                server.Release.TrySetResult(true);
                PumpUntil(() => activation.IsCompleted);
                activation.GetAwaiter().GetResult();

                Assert.True(watch.Elapsed < TimeSpan.FromSeconds(1), "The activation button blocked on the network.");
                Assert.Equal(!cancel, state.Settings.ActiveAutoSplitters.Contains("Test Game"));
                Assert.Equal(!cancel, splitter.IsActivated);
                Assert.Equal(!cancel, Field<Button>(editor, "btnSettings").Enabled);
                splitter.Deactivate();
            }
            finally
            {
                ComponentManager.BasePath = originalPath;
                typeof(ComponentManager).GetProperty(nameof(ComponentManager.ComponentFactories)).SetValue(null, originalFactories);
                typeof(AutoSplitterFactory).GetProperty(nameof(AutoSplitterFactory.Instance)).SetValue(null, original);
                Directory.Delete(directory, true);
            }
        });
    }

    private static T Field<T>(RunEditorDialog editor, string name) =>
        (T)typeof(RunEditorDialog).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(editor);

    public class ScriptFactory : IComponentFactory
    {
        public string ComponentName => "Test";
        public string Description => "Test";
        public ComponentCategory Category => ComponentCategory.Other;
        public string UpdateName => "Test";
        public string XMLURL => "";
        public string UpdateURL => "";
        public Version Version => new(1, 0);
        public int CreationThread;
        public int CreationCount;
        public RecordingComponent Created;
        public IComponent Create(LiveSplitState state) => Create(state, "");
        public IComponent Create(LiveSplitState state, string path)
        {
            CreationThread = Thread.CurrentThread.ManagedThreadId;
            CreationCount++;
            return Created = new RecordingComponent();
        }
    }

    public class RecordingComponent : SeparatorComponent, IComponent
    {
        private readonly Control settingsControl = new();
        public XmlNode Settings;
        public new void SetSettings(XmlNode settings) => Settings = settings;
        public new Control GetSettingsControl(LayoutMode mode) => settingsControl;
        public new void Dispose() => settingsControl.Dispose();
    }

    private sealed class DelayedServer : IDisposable
    {
        private readonly TcpListener listener = new(IPAddress.Loopback, 0);
        private readonly int status;
        public TaskCompletionSource<bool> Requested { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public TaskCompletionSource<bool> Release { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public string Url { get; }

        public DelayedServer(int status = 200)
        {
            this.status = status;
            listener.Start();
            Url = $"http://127.0.0.1:{((IPEndPoint)listener.LocalEndpoint).Port}/test.asl";
            _ = Task.Run(RespondAsync);
        }

        private async Task RespondAsync()
        {
            try
            {
                using TcpClient client = await listener.AcceptTcpClientAsync();
                using NetworkStream stream = client.GetStream();
                using var reader = new StreamReader(stream, Encoding.ASCII, false, 1024, true);
                while (!string.IsNullOrEmpty(await reader.ReadLineAsync())) { }
                Requested.TrySetResult(true);
                await Release.Task;
                byte[] response = Encoding.ASCII.GetBytes($"HTTP/1.1 {status} Test\r\nContent-Length: 11\r\nConnection: close\r\n\r\ntest script");
                await stream.WriteAsync(response, 0, response.Length);
            }
            catch (ObjectDisposedException) { }
            catch (SocketException) { }
            catch (IOException) { }
        }

        public void Dispose()
        {
            Release.TrySetResult(true);
            listener.Stop();
        }
    }

    private static void PumpUntil(Func<bool> condition)
    {
        DateTime deadline = DateTime.UtcNow.AddSeconds(8);
        while (!condition() && DateTime.UtcNow < deadline)
        {
            Application.DoEvents();
            Thread.Yield();
        }
        Assert.True(condition(), "The UI did not observe the background update.");
    }

    private static void OnStaThread(Action action)
    {
        Exception failure = null;
        var thread = new Thread(() =>
        {
            SynchronizationContext.SetSynchronizationContext(null);
            try { action(); }
            catch (Exception ex) { failure = ex; }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        Assert.True(thread.Join(TimeSpan.FromSeconds(15)), "UI test timed out.");
        if (failure != null)
        {
            ExceptionDispatchInfo.Capture(failure).Throw();
        }
    }
}
