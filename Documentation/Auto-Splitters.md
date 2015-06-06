# Auto Splitters

LiveSplit has integrated support for Auto Splitters. An Auto Splitter can be one of the following:
* A LiveSplit Component written in any .NET compatible language.
* A third party Application communicating with LiveSplit through LiveSplit  Server.
* A Script written in the Auto Splitting Language (ASL).

At the moment LiveSplit can automatically download and activate Auto Splitters that are LiveSplit Components or ASL Scripts. Support for Applications using LiveSplit Server is planned, but is not yet available.

## Features of an Auto Splitter

An Auto Splitter can provide any of the following features:

### Game Time

Game Time can either be Real Time without Loads or an actual Game Timer found in the game. This depends on the game and the Speedrun Community of that game. If the game has been run in Real Time for multiple years already, introducing a new Timing Method might not be worth it.

### Automatic Splits

An Auto Splitter, as the name suggests, can also provide automatic splits to increase the accuracy of the individual segment and split times. An Auto Splitter might not necessarily implement Automatic Splits for every single split available, as the Runner might want to have some additional splits that are not supported by the Auto Splitter used-

### Automatic Timer Start

Not every Auto Splitter automatically starts the Timer. For some games the Auto Splitter can't tell whether the Runner just wants to start the game to practice something or whether the runner actually wants to do a run.

### Automatic Resets
