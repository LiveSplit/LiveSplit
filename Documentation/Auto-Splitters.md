# Auto Splitters

<!-- TOC depth:6 withLinks:1 updateOnSave:1 orderedList:0 -->

- [Features of an Auto Splitter](#features-of-an-auto-splitter)
	- [Game Time](#game-time)
	- [Automatic Splits](#automatic-splits)
	- [Automatic Timer Start](#automatic-timer-start)
	- [Automatic Resets](#automatic-resets)
- [How an Auto Splitter works](#how-an-auto-splitter-works)
	- [Pointer Paths](#pointer-paths)
- [The Auto Splitting Language - ASL](#the-auto-splitting-language---asl)
	- [State Descriptors](#state-descriptors)
		- [State objects](#state-objects)
	- [Actions](#actions)
		- [Timer Control](#timer-control)
		- [Script Management](#script-management)
	- [Action Variables](#action-variables)
		- [General Variables](#general-variables)
		- [Game Dependent](#game-dependent)
	- [Settings](#settings-1)
		- [Basic Settings](#basic-settings)
		- [Custom Settings](#custom-settings)
	- [Built-in Functions](#built-in-functions)
	- [Testing your Script](#testing-your-script)
		- [Debugging](#debugging)
- [Adding an Auto Splitter](#adding-an-auto-splitter)
- [Additional Resources](#additional-resources)

<!-- /TOC -->

LiveSplit has integrated support for Auto Splitters. An Auto Splitter can be one of the following:
* A Script written in the Auto Splitting Language (ASL).
* A LiveSplit Component written in any .NET compatible language.
* A third party application communicating with LiveSplit through the LiveSplit  Server.

At the moment LiveSplit can automatically download and activate Auto Splitters that are LiveSplit Components or ASL Scripts. Support for applications using the LiveSplit Server is planned, but is not yet available.

## Features of an Auto Splitter

An Auto Splitter can provide any of the following features:

### Game Time

Game Time can either be Real Time without Loads or an actual Game Timer found in the game. This depends on the game and the speedrun community of that game. If the game has been run in Real Time for multiple years already, introducing a new timing method might not be worth it.

### Automatic Splits

An Auto Splitter, as the name suggests, can also provide automatic splits to increase the accuracy of the individual segment and split times. An Auto Splitter might not necessarily implement Automatic Splits for every single split available since the runner might want to have some additional splits that are not supported by the Auto Splitter used.

### Automatic Timer Start

Not every Auto Splitter automatically starts the timer. For some games, the Auto Splitter can't tell whether the runner just wants to start the game to practice something or actually wants to do a run.

### Automatic Resets

An Auto Splitter can automatically reset the timer. This might be useful for games where going back to the main menu always means that the run is over. This is a bit dangerous though, as the runner might not have wanted to reset there. Think twice before implementing this functionality into your Auto Splitter.

## How an Auto Splitter works

Auto Splitters can work in one or multiple of the following ways:
 * It can read the RAM, interpret the values, and toggle any of the actions described above based on these values. The RAM addresses might not always be the same. Therefore, the Auto Splitter might need to follow a so called *Pointer Path*. This is what most Auto Splitters do and what is directly supported by the Auto Splitting Language.
 * It can scan the game's RAM to find a value and toggle any of the actions based on these values. Doing a memory scan is slower than following a Pointer Path.
 * It can read the game's log files, parse them, and toggle actions based on those. This works, but is usually fairly delayed, which is why this isn't that great of a solution.
 * It can read the game's standard output stream. This only works if the game actually writes valuable information to the standard output, which is rare. Also, Steam prevents this method since in order to read the standard output, you need to start the game's process through the Auto Splitter, which Steam won't let you do.

### Pointer Paths

A Pointer Path is a list of Offsets + a Base Address. The Auto Splitter reads the value at the base address and interprets the value as yet another address. It adds the first offset to this address and reads the value of the calculated address. It does this over and over until there are no more offsets. At that point, it has found the value it was searching for. This resembles the way objects are stored in memory. Every object has a clearly defined layout where each variable has a consistent offset within the object, so you basically follow these variables from object to object.

*Cheat Engine* is a tool that allows you to easily find Addresses and Pointer Paths for those Addresses, so you don't need to debug the game to figure out the structure of the memory.

## The Auto Splitting Language - ASL

The Auto Splitting Language is a small scripting language made specifically for LiveSplit Auto Splitters. It has a few advantages and disadvantages over normal Components.

**Advantages:**
 * ASL Scripts are easy to maintain.
 * There's no need to update the Script for new LiveSplit versions.
 * No Visual Studio or any compiler is needed; you can write it in Notepad.

**Disadvantages:**
 * Currently only provides Boolean settings in the GUI for the user to change.

An ASL Script contains a State Descriptor and multiple [Actions](#actions) which contain C# code.

### State Descriptors

The State Descriptor is the most important part of the script and describes which game process and which state of the game the script is interested in. This is where all of the Pointer Paths, which the Auto Splitter uses to read values from the game, are described. A State Descriptor looks like this:
```
state("PROCESS_NAME")
{
	POINTER_PATH
	POINTER_PATH
	...
}
```
If the script needs to support multiple versions of the game, you can specify an optional version identifier:
```
state("PROCESS_NAME", "VERSION_IDENTIFIER")
...
```

The `PROCESS_NAME` is the name of the process the Auto Splitter should look for. The Script is inactive while it's not connected to a process. Once a process with that name is found, it automatically connects to that process. A Process Name should not include the `.exe`. Even advanced scripts that use other ways to access the game's memory require a State Descriptor to define which process LiveSplit is supposed to connect to.

The optional `VERSION_IDENTIFIER` can be any arbitrary string you wish to use. Note that the script can define multiple State Descriptors for different processes/games. These optional features are extremely useful for emulators.

`POINTER_PATH` describes a Pointer Path and has two ways to declare:
```
VARIABLE_TYPE VARIABLE_NAME : OFFSET, OFFSET, OFFSET, ...;
VARIABLE_TYPE VARIABLE_NAME : "BASE_MODULE", OFFSET, OFFSET, OFFSET, ...;
```

The variable type `VARIABLE_TYPE` describes the type of the value found at the pointer path. It can be one of the following:

| Type             | Description                |
|------------------|----------------------------|
| sbyte            | Signed 8-bit integer       |
| byte             | Unsigned 8-bit integer     |
| short            | Signed 16-bit integer      |
| ushort           | Unsigned 16-bit integer    |
| int              | Signed 32-bit integer      |
| uint             | Unsigned 32-bit integer    |
| long             | Signed 64-bit integer      |
| ulong            | Unsigned 64-bit integer    |
| float            | 32-bit IEEE floating-point |
| double           | 64-bit IEEE floating-point |
| bool             | Boolean                    |
| string\<length\> | String (e.g. string255)    |
| byte\<length\>   | Byte array (e.g. byte255)  |

The variable name `VARIABLE_NAME` can be any variable name you choose, describing what is found at the pointer path. The naming is up to you, but should be distinct from the other variable names.

The optional base module name `BASE_MODULE` describes the name of the module the Pointer Path starts at. Every \*.exe and \*.dll file loaded into the process has its own base address. Instead of specifying the base address of the Pointer Path, you specify the base module and an offset from there. If this is not defined, it will default to the main (.exe) module.

You can use as many offsets `OFFSET` as you want. They need to be integer literals, either written as decimal or hexadecimal.

#### State objects

The State Variables described through the State Descriptor are available through two State objects: `current` and `old`. The `current` object contains the current state of the game with all the up-to-date variables, while the `old` object contains the state of the variables at the last execution of the ASL Script in LiveSplit. These objects are useful for checking for state changes. For example, you could check if the last level of a game was a certain value and is now a certain other value, which might mean that a split needs to happen.

LiveSplit's internal state is also available through the object `timer`. This is an object of the type [`LiveSplitState`](../LiveSplit/LiveSplit.Core/Model/LiveSplitState.cs) and can be used to interact with LiveSplit in ways that are not directly available through ASL.

### Actions

After writing a State Descriptor, you can implement multiple actions such as splitting and starting the timer. These actions define the logic of the Auto Splitter based on the information described by the State Descriptor. An action looks like this:
```
ACTION_NAME
{
	C# CODE
}
```

You can think of Actions like functions that are automatically called by the ASL Component. These functions can only interact with each other or LiveSplit via the [special variables](#action-variables) the environment provides.

All of the actions are optional and are declared by their name `ACTION_NAME` followed by a code block `CODE`. You trigger the action by returning a value. Returning a value is optional though; if no value is returned, the action is not triggered. Some actions are only executed while LiveSplit is connected to the process.

Actions are implemented in C#. You can use C#'s documentation for any questions you may have regarding the syntax of C#.

#### Timer Control

These actions are repeatedly triggered while LiveSplit is connected to the game process.

##### Generic Update

The name of this action is `update`. You can use this for generic updating. In each update iteration, this is run before the timer control actions, which e.g. means if you set a value in `vars` in `update` you can then access it in `start` on the same update cycle.

Explicitly returning `false` will prevent the actions `isLoading`, `gameTime`, `reset`, `split`, and `start` from being run. This can be useful if you want to entirely disable the script under some conditions (e.g. for incompatible game versions). See [Order of Execution](#order-of-execution) for more information.

##### Automatic Timer Start

The name of this action is `start`. Return `true` whenever you want the timer to start. Note that the `start` action will only be run if the timer is currently not running.

##### Automatic Splits

The name of this action is `split`. Return `true` whenever you want to trigger a split.

##### Automatic Resets

The name of this action is `reset`. Return `true` whenever you want to reset the run.

Explicitly returning `true` will prevent the `split` action from being run. This can be useful in some cases, but may also cause issues for some scripts. See [Order of Execution](#order-of-execution) for more information.

##### Load Time Removal

The name of this action is `isLoading`. Return `true` whenever the game is loading. LiveSplit's Game Time Timer will be paused as long as you return `true`.

**NOTE**: Make sure the timer is set to "Game Time" in the layout! Failure to do so will cause the timer to keep running, as if `isLoading` had returned `false` or `isLoading` weren't triggered at all.

##### Game Time

The name of this action is `gameTime`. Return a [`TimeSpan`](https://msdn.microsoft.com/en-us/library/system.timespan(v=vs.110).aspx) object that contains the current time of the game. You can also combine this with `isLoading`. If `isLoading` returns false, nothing, or isn't implemented, LiveSplit's Game Time Timer is always running and syncs with the game's Game Time at a constant interval. Everything in between is therefore a Real Time approximation of the Game Time. If you want the Game Time to not run in between the synchronization interval and only ever return the actual Game Time of the game, make sure to implement `isLoading` with a constant return value of `true`.

##### Order of Execution

Understanding the order and conditions under which timer control actions are executed can help you avoid issues in your script where variables appear to be set improperly, actions appear to be skipped, and more. Every update iteration follows this process when running actions:

1. `update` will always be run first. There are no conditions on the execution of this action.
2. If `update` did not explicitly return `false` and the timer is currently either running or paused, then the `isLoading`, `gameTime`, and `reset` actions will be run.
  - If `reset` does not explicitly return `true`, then the `split` action will be run.
3. If `update` did not explicitly return `false` and the timer is currently not running (and not paused), then the `start` action will be run.

#### Script Management

##### Script Startup

The name of this action is `startup`. This action is triggered when the script is first loads. This is the place where you can put initialization that doesn't depend on being connected to the process and the only place where you can add [Custom Settings](#custom-settings).

##### Script Shutdown

The name of this action is `shutdown`. This action is triggered whenever the script is entirely stopped, for example when the Auto Splitter is disabled, LiveSplit exits, the script path is changed or the script is reloaded (e.g. during development of the ASL script).

##### Script Initialization (Game Start)

The name of this action is `init`. This action is triggered whenever a game process has been found according to the State Descriptors. This can occur more than once during the execution of a script (e.g. when you restart the game). This is the place to do initialization that depends on the game, for example detecting the game version.

##### Game Exit

The name of this action is `exit`. This action is triggered whenever the currently attached game process exits.


### Action Variables

Actions have a few hidden variables available.

#### General Variables

##### vars
A dynamic object which can be used to store variables. Make sure the variables are defined (for example in `startup` or `init`) before trying to access them. This can be used to exchange data between Actions.
```
init { vars.test = 5; }
update { print(vars.test.ToString()); }
```
You can also store variables like this in `current` and the value will be in `old` on the next update.

##### version
When you set `version` in `init`, the corresponding State Descriptor will be activated. When there is no State Descriptor corresponding to the `version`, the default one will be activated.

The default is the first defined State Descriptor with no version specified, or the first State Descriptor in the file if there is none with no version specified.

The string you set `version` to will also be displayed in the ASL Settings GUI.
```
state("game", "v1.2")
{
    byte levelID : 0x9001;
}
state("game", "v1.3")
{
    byte levelID : 0x9002;
}
init
{
    if (modules.First().ModuleMemorySize == 0x123456)
        version = "v1.2";
    else if (modules.First().ModuleMemorySize == 0x654321)
        version = "v1.3";
}
update
{
    if (version == "") return false;
    print(current.levelID.ToString());
}
```

##### refreshRate
Many actions are triggered repeatedly, by default approximately 60 times per second. You can set this variable lower to reduce CPU usage. This is commonly done in `startup` or `init`.
```
refreshRate = 30;
```

##### settings
Used to add and access [Settings](#settings-1).

#### Game Dependent

These variables depend on being or having been connected to a game process and are not available in the `startup` or `exit` actions and only partly available in `shutdown` (might be `null`).

##### current / old
State objects representing the current and previous states.
```
split { return current.levelID != old.levelID; }
```

##### game
The currently connected [Process](https://msdn.microsoft.com/en-us/library/system.diagnostics.process%28v=vs.110%29.aspx) object.
```
update { if (game.ProcessName == "snes9x") { } }
```

##### modules
The modules of the currently connected process. Please use this instead of game.Modules! Use modules.First() instead of game.MainModule.

##### memory
Provides a means to read memory from the game without using the State Descriptor.
```
vars.exe = memory.ReadBytes(modules.First().BaseAddress, modules.First().ModuleMemorySize);
vars.test = memory.ReadValue<byte>(modules.First().BaseAddress + 0x9001);
vars.test2 = memory.ReadString(modules.First().BaseAddress + 0x9002, 256);
vars.test3 = new DeepPointer("some.dll", 0x9003, vars.test, 0x02).Deref<int>(game);
vars.test4 = memory.ReadString(modules.Where(m => x.ModuleName == "some.dll").First().BaseAddress + 0x9002, 256);
```

### Settings

ASL script settings are stored either with the Layout if you are using the Scriptable Auto Splitter component or with the Splits if you activated the script in the Splits Editor (so make sure to save your Layout or Splits accordingly when exiting LiveSplit if you changed settings).

#### Basic Settings
The Auto Splitter Settings GUI has some default settings to allow the user to toggle the actions `start`, `reset` and `split`. If the checkbox for an action is unchecked, the return value of that action is ignored (but the code inside the action is still executed). So for example if the checkbox for `start` is unchecked, returning `true` in the `start` action will have no effect.

You can access the current value of the basic settings through the attributes `settings.StartEnabled`, `settings.ResetEnabled` and `settings.SplitEnabled`. This is only for informational purposes, for example if your script needs to do something depending on whether the action was actually performed or not, ignoring the return value is done automatically.

Actions that are not present in the ASL script or empty will have their checkboxes disabled.

#### Custom Settings

You can define custom boolean settings for your script in the `startup` action and then access the setting values as configured by the user in the other actions. If you have custom settings defined, they will be shown in the GUI for the user to check/uncheck. They will appear in the same order you added them in the ASL script.

##### Basic usage

You can define settings in the `startup` action by using the `settings.Add(id, default_value = true, description = null, parent = null)` method:

```
// Add setting 'mission1', enabled by default, with 'First Mission' being displayed in the GUI
settings.Add("mission1", true, "First Mission");

// Add setting 'mission2', enabled by default, with 'mission2' being displayed in the GUI
settings.Add("mission2");

// Add setting 'mission3', disabled by default, with 'mission3' being displayed in the GUI
settings.Add("mission3", false);
```

You can access the current value of a setting in all actions other than `startup` by accessing `settings`:

````
// Do something depending on the value of the setting 'mission1'
if (settings["mission1"]) {  }
````

##### Grouping settings
If you want to organize the settings in a hierarchy, you can specify the `parent` parameter. Note that the `parent` has to be the `id` of a setting that you already added:

```
// Add setting 'main_missions'
settings.Add("main_missions", true, "Main Missions");

// Add setting 'mission1', with 'main_missions' as parent
settings.Add("mission1", true, "First Mission", "main_missions");
```

Settings only return `true` (checked) when their `parent` setting is `true` as well. The user can still freely toggle settings that have their parent unchecked, however they will be grayed out to indicate they are disabled.

Any setting can act as a `parent` setting, so you could for example do the following to go one level deeper (continuing from the last example):

```
// Add setting 'mission1_part1', with 'mission1' as parent
settings.Add("mission1_part1", true, "First part of Mission 1", "mission1");
```

The setting `mission1_part1` will only be enabled, when both `mission1` and `main_missions` are enabled.

When the `parent` parameter is null or omitted, the setting will be added as top-level setting, unless `settings.CurrentDefaultParent` is set to something other than `null`:

```
// Add top-level setting 'main_missions'
settings.Add("main_missions");

settings.CurrentDefaultParent = "main_missions";

// Add setting 'mission1', with the parent 'main_missions'
settings.Add("mission1");

settings.CurrentDefaultParent = null;

// Add top-level setting 'side_missions'
settings.Add("side_missions");
```

Using `settings.CurrentDefaultParent` can be useful when adding several settings with the same parent, without having to specify the parent every time.

##### Tooltips

You can add a tooltip to settings that appears when hover over the setting in the GUI. This can be useful if you want to add a bit more information. After adding the setting, use `settings.SetToolTip(id, tooltip_text)` to set a tooltip:

```
settings.Add("main_missions", true, "Main Missions");
settings.SetToolTip("main_missions", "All main story missions, except Mission A and Mission B");
```

### Built-in Functions

##### print
Used for debug printing. Use [DbgView](https://technet.microsoft.com/en-us/Library/bb896647.aspx) to watch the output.
```
print("current level is " + current.levelID);
```

##### Other
There are some advanced memory utilities not covered here. You can find them [here](../LiveSplit/LiveSplit.Core/ComponentUtil).

### Testing your Script

You can test your Script by adding the *Scriptable Auto Splitter* component to your Layout. Right-click on LiveSplit and choose "Edit Layout..." to open the Layout Editor, then click on the Plus-sign and choose "Scriptable Auto Splitter" from the section "Control". You can set the Path of the Script by going into the component settings of the Scriptable Auto Splitter. To get to the settings of the component you can either double click it in the Layout Editor or go into to the Scriptable Auto Splitter Tab of the Layout Settings. Once you've set the Path, the script should automatically load and hopefully work.

#### Debugging
Reading debug output is an integral part of developing ASL scripts, both for your own debug messages which you can output with `print()` and any debug messages or error messages the ASL Component itself provides.

The program [DebugView](https://technet.microsoft.com/en-us/sysinternals/debugview.aspx) can be used for a live view of debug output from the ASL Component.

For errors, you can also check the Windows Event Logs, which you can find via:

Control Panel ➞ Search for Event Viewer ➞ Open it ➞ Windows Logs ➞ Application ➞ Find the LiveSplit Errors

Some might be unrelated to the Script, but it'll be fairly obvious which ones are caused by you.

## Adding an Auto Splitter

If you implemented an Auto Splitter and want to add it to the Auto Splitters that are automatically being downloaded by LiveSplit, feel free to add it to the [Auto Splitters XML](../LiveSplit.AutoSplitters.xml). Just click the link, click the icon for modifying the file and GitHub will automatically create a fork, branch and pull request for you, which we can review and then merge in.

## Additional Resources

- [Speedrun Tool Development Discord](https://discord.gg/N6wv8pW)
- [List of ASL Scripts](https://fatalis.pw/livesplit/asl-list/) to learn from, automatically created from the [Auto Splitters XML](../LiveSplit.AutoSplitters.xml) and filterable by different criteria
- Example: [Simple Autosplitter with Settings](https://raw.githubusercontent.com/tduva/LiveSplit-ASL/master/AlanWake.asl)
