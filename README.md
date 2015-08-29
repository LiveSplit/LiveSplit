<h1> <img src="https://raw.githubusercontent.com/LiveSplit/LiveSplit/master/LiveSplit/Resources/Icon.png" alt="LiveSplit" height="42" width="45" align="top"/> LiveSplit</h1>

[![GitHub release](https://img.shields.io/github/release/LiveSplit/LiveSplit.svg)](https://github.com/LiveSplit/LiveSplit/releases/latest)
[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://raw.githubusercontent.com/LiveSplit/LiveSplit/master/LICENSE)
[![Build status](https://ci.appveyor.com/api/projects/status/o6kwuifjotbo6t5a/branch/master?svg=true)](https://ci.appveyor.com/project/CryZe/livesplit/branch/master)

[![GitHub issues](https://img.shields.io/github/issues/LiveSplit/LiveSplit.svg?style=plastic)](https://github.com/LiveSplit/LiveSplit/issues)
[![Stories in Ready](https://badge.waffle.io/LiveSplit/LiveSplit.png?label=ready&title=Ready)](https://waffle.io/LiveSplit/LiveSplit)
[![Stories in Progress](https://badge.waffle.io/LiveSplit/LiveSplit.png?label=in progress&title=In Progress)](https://waffle.io/LiveSplit/LiveSplit)

LiveSplit is a timer program for speedrunners that is both easy to use and full of features.
<p align="center">
  <img src="http://livesplit.org/wp-content/uploads/2014/01/livesplit_1.41.png" alt="LiveSplit"/>
</p>

## Features

**Speedrun.com Integration:** [Speedrun.com](http://speedrun.com) is fully integrated into LiveSplit. You can browse their Leaderboards, download splits and even submit your own runs directly from within LiveSplit, simplifying the process a lot. You can even show the World Record of the Games you run with the World Record Component.

**Accurate Timing:** LiveSplit automatically synchronizes with an Atomic Clock over the Internet to estimate inaccuracies of the local timer in the PC. LiveSplit's timer automatically adjusts the local timer to fix those inaccuracies.

**Game Time and Auto Splitting:** LiveSplit will automatically detect if Game Time and/or Auto Splitting is available for a game and lets you activate it in the Splits Editor. Game Time is automatically being read directly from an emulator or PC game. You can use it by switching to Game Time under Compare Against.

**Video Component:** With the Video Component, you can play a video from a local file alongside your run. The video will start when you start your run and stop whenever you reset. You can also specify at what point the video should start at.

**SpeedRunsLive Racing:** In LiveSplit, you are able to start and join races on [SpeedRunsLive](http://www.speedrunslive.com/) within LiveSplit itself. The timer automatically starts when the race begins and automatically writes ``.done`` whenever you complete the race. Also, you are able to compare your current run with the other runners during the race, as long as they use LiveSplit as well.

**Comparisons:** In LiveSplit, you are able to dynamically switch between multiple comparisons, even mid-run. You can either compare your run to comparisons that you define yourself or compare it to multiple automatically generated comparisons, like your sum of best or your average run. While racing on [SpeedRunsLive](http://www.speedrunslive.com/), comparisons for the other runners are automatically generated as well.

**Layout System:** Users can modify every part of LiveSplit’s appearance using layouts. Every user has the ability to add or remove parts along with being able to rearrange and customize each part of LiveSplit. You can even define your own background images.

**Dynamic Resizing:** LiveSplit can be resized to any size so that it looks good on stream. As LiveSplit’s size is changed, all of its parts are automatically scaled up in order to preserve its appearance.

**Sharing Runs:** Any run can be shared to websites such as [Speedrun.com](http://speedrun.com/), [Congratsio](http://www.congratsio.com/) and [Twitter](https://twitter.com/). Splits can also be distributed using [splits i/o](https://splits.io/) and imported from a URL. You can also share a screenshot of your splits to [Imgur](http://imgur.com/) or save it as a file. Your [Twitch](http://www.twitch.tv/) title can be updated as well based on the game you are playing.

**Gamepad Support:** You can easily use gamepads, steering wheels, foot pedals, special mouse keys for controlling LiveSplit if you want to. You don’t need to simulate keyboard keys to use them. It just works.

**Component Development:** Anyone can develop their own components that can easily be shared and used inside LiveSplit. Additional components that will be released can be found in the [Components Section](http://livesplit.org/components/).

## Contributing

We need your help!

You can browse the [Issues](https://waffle.io/LiveSplit/LiveSplit) to find good issues to get started with. Select one that is not already done or in progress, assign yourself and drag it over to "In Progress".

 1. [Fork](https://github.com/LiveSplit/LiveSplit/fork) the project
 2. Clone your forked repo: `git clone https://github.com/YourUsername/LiveSplit.git`
 3. Create your feature/bugfix branch: `git checkout -b new-feature`
 4. Commit your changes to your new branch: `git commit -am 'Add a new feature'`
 5. Push to the branch: `git push origin new-feature`
 6. Create a new Pull Request!

## Compiling

LiveSplit is written in C# 5 with Visual Studio and uses .NET Framework 4.0. To compile LiveSplit you can get any version of Visual Studio that supports .NET Framework 4.0, these include:
 - Visual C# 2010 Express
 - Visual Studio 2010
 - Visual Studio 2013 Express for Windows Desktop
 - Visual Studio 2013 Community Edition
 - Visual Studio 2013
 - Visual Studio 2015 Community Edition
 - Visual Studio 2015

Preferably you should either get a full version of Visual Studio 2015 or the Community Edition if you don't want to pay for Visual Studio.

Simply open the project with Visual Studio and it should be able to compile and run it without any further configuration.

## Common Compiling Issues
1. Could not build Codaxy.Xlio due to sgen.exe not being found. Open LiveSplit\\Libs\\xlio\\Source\\Codaxy.Xlio\\Codaxy.Xlio.csproj in order to edit where it looks for this path. Look for &lt;SGen...&gt; where it defines the attribute "ToolPath". Look on your computer to find the proper path. It is typically down some path such as "C:\\Program Files (x86)\\Microsoft SDKs\\Windows\\x.xA...". Find the version you want to use and bin folder with sgen.exe in it and replace the path in the .csproj file.
2. No submodules pulled in when you fork/clone the repo which causes the project not to build. There are two ways to remedy this:
 - Cloning for the first time: `git clone --recursive git://repo/repo.git`
 - If already cloned, execute this in the root directory: `git submodule update --init --recursive`

## Auto Splitters

The Documentation about how to develop, test and submit an Auto Splitter can be found here:

[Auto Splitters Documentation](Documentation/Auto-Splitters.md)

## License

The MIT License (MIT)

Copyright (c) 2015 Christopher Serr and Sergey Papushin

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
