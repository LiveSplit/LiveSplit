# LiveSplit Repository Guide

## Project

LiveSplit is a Windows desktop speedrun timer built with C# and WinForms. The application targets .NET Framework 4.8.1 and uses C# 14.
The repository is a superproject: most UI components and the native timing core live in Git submodules.

## Required environment

- Windows
- .NET 10 SDK
- .NET Framework 4.8.1 Developer Pack
- Git with submodule support

The repository is Windows-only. Do not assume cross-platform build or runtime behavior.

## Bootstrap

Always initialize submodules before restoring or building:

```powershell
git submodule update --init --recursive
```

Components and libraries are separate repositories. After switching branches or pulling commits that update submodule pointers, run the command again.

## Build and test

Build the main solution in Release configuration:

```powershell
dotnet build -v m -c Release -p:DebugType=None .\LiveSplit.slnx
```

Build the standalone updater executable:

```powershell
dotnet build -v m -c UpdateManagerExe -p:DebugType=None .\src\UpdateManager\UpdateManager.csproj
```

Run tests after building the solution:

```powershell
dotnet test -v m -c Release --no-restore --no-build .\LiveSplit.slnx
```

Build output is written to `artifacts/build/<configuration>`. Test binaries are written under `artifacts/bin`. These directories are ignored and must not be committed.

## Architecture map

- `src/LiveSplit/Program.cs`: application entry point. Parses `-s` (splits file) and `-l` (layout file), initializes localization, and opens `TimerForm`.
- `src/LiveSplit.Core/`: domain and runtime logic.
  - `Model/`: `LiveSplitState`, `TimerModel`, runs, segments, comparisons, timing, input, and autosplitter model logic.
  - `UI/`: layout model and component contracts (`IComponent`, `IComponentFactory`, `Layout`, `ComponentManager`).
  - `Options/`: settings, hotkeys, layout settings, and persistence support.
  - `Server/`: named pipe, TCP, and WebSocket command server.
  - `TimeFormatters/`: timer and delta text formatting.
  - `Web/`: speedrun.com, SRL, sharing, and credential integrations.
  - `ComponentUtil/`: memory reading, pointer chains, and signature scanning helpers.
- `src/LiveSplit.View/`: WinForms views and dialogs, centered on `View/TimerForm.cs`.
- `components/`: built-in UI components as Git submodules. Each normally contains `src/<ComponentName>/` and a project with `EnableDynamicLoading`.
- `lib/livesplit-core/`: Rust native timing core submodule. The checked-in `src/LiveSplit.Core/LiveSplitCore.g.cs` is generated P/Invoke bindings; the app loads `x64/livesplit_core.dll` or `x86/livesplit_core.dll` at runtime.
- `test/LiveSplit.Tests/`: xUnit test project covering the model, formatting, settings, parsing, localization, and selected components.

## Runtime flow

1. `Program.Main` initializes localization and creates `TimerForm`.
2. `TimerForm` creates a `LiveSplitState`, wraps `TimerModel` with `DoubleTapPrevention`, loads settings/layout/splits, and discovers component factories with `ComponentManager`.
3. A layout contains `ILayoutComponent` instances backed by `IComponent` implementations from `Components/*.dll`.
4. UI updates render against the current `LiveSplitState`; timer transitions are driven through `ITimerModel`.
5. The native timing core is accessed through the generated `LiveSplitCore` bindings and checked-in architecture-specific DLLs.

## Component rules

- A component is a separate repository under `components/`, not ordinary main-repository source.
- Implement `IComponent` for rendering/settings and `IComponentFactory` for discovery and metadata.
- Mark the factory assembly with `[assembly: ComponentFactory(typeof(YourFactory))]`.
- Component projects reference `LiveSplit.Core` with `Private="false"` and `ExcludeAssets="runtime"` so runtime assemblies are shared with the host.
- Keep component settings backward-compatible. `GetSettings`/`SetSettings` serialize user data to XML and should tolerate older or missing nodes.
- When changing a submodule, commit in the submodule repository first, then update the superproject's submodule pointer.

## Coding conventions

- Follow `.editorconfig`; build-time code style enforcement is enabled.
- C# uses 4-space indentation, CRLF line endings, and UTF-8 BOM.
- Project/props files use 2-space indentation.
- Nullable reference types are disabled.
- Prefer file-scoped namespaces and the existing modern C# style, including collection expressions.
- Do not edit generated native bindings or checked-in native DLLs unless the corresponding `livesplit-core` change and regeneration workflow is part of the task.
- Preserve WinForms designer compatibility. Avoid unnecessary regeneration of `*.Designer.cs` and `*.resx` files.
- Because LiveSplit is timer software, be conservative with state transitions, timing precision, persistence formats, and pause/reset behavior. Add focused regression tests for behavioral changes.

## Testing guidance

- Existing tests use xUnit and descriptive `Must` naming conventions in several model test classes.
- Add tests near the affected domain area: `Model`, `TimeFormatters`, `Options`, `Localization`, `AutoSplitterTests`, or `UI` serialization tests.
- For pure logic changes, prefer unit tests over manual GUI verification.
- For layout/component changes, verify both XML round-tripping and rendering settings behavior where practical.