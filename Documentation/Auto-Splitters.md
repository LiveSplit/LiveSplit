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
		- [Automatic Timer Start](#automatic-timer-start-1)
		- [Automatic Splits](#automatic-splits-1)
		- [Automatic Resets](#automatic-resets-1)
		- [Load Time Removal](#load-time-removal)
		- [Game Time](#game-time-1)
	- [Testing your Script](#testing-your-script)
- [Adding an Auto Splitter](#adding-an-auto-splitter)

<!-- /TOC -->

LiveSplit has integrated support for Auto Splitters. An Auto Splitter can be one of the following:
* A LiveSplit Component written in any .NET compatible language.
* A third party application communicating with LiveSplit through the LiveSplit  Server.
* A Script written in the Auto Splitting Language (ASL).

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
 * Pointer Paths need to be defined as Constants, which means only one version of the game can be supported.
 * Only Pointer Paths are available; no Memory Scans can be done.

An Auto Splitting Language Script contains a State Descriptor and multiple Actions.

### State Descriptors

The State Descriptor is the most important part of the script and describes the state of the game that the script is interested in. This is where all of the Pointer Paths, which the Auto Splitter uses to read values from the game, are described. A State Descriptor looks like this:
```
state("PROCESS_NAME")
{
	POINTER_PATH
	POINTER_PATH
	...
}
```

The `PROCESS_NAME` is the name of the process the Auto Splitter should look for. The Script is inactive while it's not connected to a process. Once a process with that name is found, it automatically connects to that process. A Process Name should not include the `.exe`.

`POINTER_PATH` describes a Pointer Path and looks like this:
```
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

The base module name `BASE_MODULE` describes the name of the module the Pointer Path starts at. Every \*.exe and \*.dll file loaded into the process has its own base address. Instead of specifying the base address of the Pointer Path, you specify the base module and an offset from there.

You can use as many offsets `OFFSET` as you want. They need to be integer literals, either written as decimal or hexadecimal.

### State objects

The State Variables described through the State Descriptor are available through two State objects: `current` and `old`. The `current` object contains the current state of the game with all the up-to-date variables, while the `old` object contains the state of the variables at the last execution of the ASL Script in LiveSplit. These objects are useful for checking for state changes. For example, you could check if the last level of a game was a certain value and is now a certain other value, which might mean that a split needs to happen. Both the `current` and `old` objects are also completely dynamic, which means that you can store any of your own values in them.

For example, you can just do the following:
```
current.myCustomValue = 5;
```

Make sure not to access values that haven't been created at that point in time though, as this might cause your script to crash.

LiveSplit's internal state is also available through the object `timer`. This is an object of the type `LiveSplitState` and can be used to interact with LiveSplit in ways that are not directly available through ASL.

### Actions

After writing a State Descriptor, you can implement multiple Actions such as splitting and starting the timer. These actions define the logic of the Auto Splitter based on the information described by the State Descriptor. An action looks like this:
```
ACTION_NAME
{
	CODE
}
```

All of the actions are optional and are declared by their name `ACTION_NAME` followed by a code block `CODE`. You trigger the action by returning a value. Returning a value is optional though; if no value is returned, the action is not triggered. Actions are only executed while LiveSplit is connected to the process. Their implementation is written in C#. You can use C#'s documentation for any questions you may have regarding the syntax of C#.

#### Automatic Timer Start

The name of this action is `start`. Return `true` whenever you want the timer to start.

#### Automatic Splits

The name of this action is `split`. Return `true` whenever you want to trigger a split.

#### Automatic Resets

The name of this action is `reset`. Return `true` whenever you want to reset the run.

#### Load Time Removal

The name of this action is `isLoading`. Return `true` whenever the game is loading. LiveSplit's Game Time Timer will be stopped as long as you return `true`.

#### Game Time

The name of this action is `gameTime`. Return a [`TimeSpan`](https://msdn.microsoft.com/en-us/library/system.timespan(v=vs.110).aspx) object that contains the current time of the game. You can also combine this with `isLoading`. If `isLoading` returns false, nothing, or isn't implemented, LiveSplit's Game Time Timer is always running and syncs with the game's Game Time at a constant interval. Everything in between is therefore a Real Time approximation of the Game Time. If you want the Game Time to not run in between the synchronization interval and only ever return the actual Game Time of the game, make sure to implement `isLoading` with a constant return value of `true`.

### Testing your Script

You can test your Script by adding the *Scriptable Auto Splitter* component to your Layout. You can find it in the section "Control". You can set the Path of the Script by going into the component settings of the Scriptable Auto Splitter. To get to the settings of the component you can either double click it in the Layout Editor or go into to the Scriptable Auto Splitter Tab of the Layout Settings. Once you've set the Path, the script should automatically load and hopefully work. Unfortunately there's no good way to debug the script at the moment. If LiveSplit causes a lot CPU Usage, that might be an indicator of something causing an exception in your Script. In that case you should check out your Event Logs. You can find it by doing the following:

Control Panel ➞ Search for Event Viewer ➞ Open it ➞ Windows Logs ➞ Application ➞ Find the LiveSplit Errors

Some might be unrelated to the Script, but it'll be fairly obvious which ones are caused by you.

## Adding an Auto Splitter

If you implemented an Auto Splitter and want to add it to the Auto Splitters that are automatically being downloaded by LiveSplit, feel free to add it to the [Auto Splitters XML](../LiveSplit.AutoSplitters.xml). Just click the link, click the icon for modifying the file and GitHub will automatically create a fork, branch and pull request for you, which we can review and then merge in.
