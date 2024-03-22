<h1> <img src="https://raw.githubusercontent.com/LiveSplit/LiveSplit/master/LiveSplit/Resources/Icon.png" alt="LiveSplit" height="42" width="45" align="top"/> LiveSplit</h1>

[![GitHub release](https://img.shields.io/github/release/LiveSplit/LiveSplit.svg)](https://github.com/LiveSplit/LiveSplit/releases/latest)
[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://raw.githubusercontent.com/LiveSplit/LiveSplit/master/LICENSE)
[![Build Status](https://github.com/LiveSplit/LiveSplit/workflows/CI/badge.svg)](https://github.com/LiveSplit/LiveSplit/actions)
[![GitHub issues](https://img.shields.io/github/issues/LiveSplit/LiveSplit.svg?style=plastic)](https://github.com/LiveSplit/LiveSplit/issues)

LiveSplit is a timer program for speedrunners that is both easy to use and full of features.
<p align="center">
  <img src="https://raw.githubusercontent.com/LiveSplit/LiveSplit.github.io/master/images/livesplittimer.png" alt="LiveSplit"/>
</p>

## Features

**Speedrun.com Integration:** [Speedrun.com](http://speedrun.com) is fully integrated into LiveSplit. You can browse their leaderboards, download splits, and even submit your own runs directly from LiveSplit. You can also show the World Records for the games you run with the World Record Component.

**Accurate Timing:** LiveSplit automatically synchronizes with an atomic clock over the Internet to estimate inaccuracies of the local timer in the PC. LiveSplit's timer automatically adjusts the local timer to fix those inaccuracies.

**Game Time and Auto Splitting:** LiveSplit will automatically detect if Game Time and/or Auto Splitting is available for a game and let you activate it in the Splits Editor. Game Time is automatically read directly from an emulator or PC game, and you can use it by switching to Game Time under Compare Against.

**Video Component:** With the Video Component, you can play a video from a local file alongside your run. The video will start when you start your run and stop whenever you reset. You can also specify at what point the video should start at.

**Racing:** In LiveSplit, you are able to start and join races on [SpeedRunsLive](http://www.speedrunslive.com/) or [racetime.gg](https://racetime.gg/) within LiveSplit itself. The timer automatically starts when the race begins and automatically writes ``.done`` whenever you complete the race. Also, you are able to compare your current run with the other runners during the race, as long as they use LiveSplit as well.

**Comparisons:** In LiveSplit, you are able to dynamically switch between multiple comparisons, even mid-run. You can either compare your run to comparisons that you define yourself or compare it to multiple automatically generated comparisons, like your Sum of Best Segments or your average run. While racing on [SpeedRunsLive](http://www.speedrunslive.com/), comparisons for the other runners are automatically generated as well.

**Layout System:** Users can modify every part of LiveSplit’s appearance using Layouts. Every user has the ability to add or remove parts along with being able to rearrange and customize each part of LiveSplit. You can even use your own background images.

**Dynamic Resizing:** LiveSplit can be resized to any size so that it looks good on stream. As LiveSplit’s size is changed, all of its parts are automatically scaled up in order to preserve its appearance.

**Sharing Runs:** Any run can be shared to websites such as [Speedrun.com](http://speedrun.com/), [Congratsio](http://www.congratsio.com/) and [Twitter](https://twitter.com/). Splits can also be distributed using [splits i/o](https://splits.io/) and imported from a URL. You can also share a screenshot of your splits to [Imgur](http://imgur.com/) or save it as a file. Your [Twitch](http://www.twitch.tv/) title can be updated as well based on the game you are playing.

**Component Development:** Anyone can develop their own components that can easily be shared and used with LiveSplit. Additional downloadable components can be found in the [Components Section](https://livesplit.org/components/).

## Contributing

We need your help!

You can browse the [Issues](https://github.com/LiveSplit/LiveSplit/issues) to find good issues to get started with. Select one that is not already done or in progress, assign yourself, and drag it over to "In Progress".

 1. [Fork](https://github.com/LiveSplit/LiveSplit/fork) the project
 2. Clone your forked repo: `git clone https://github.com/YourUsername/LiveSplit.git`
 3. Create your feature/bugfix branch: `git checkout -b new-feature`
 4. Commit your changes to your new branch: `git commit -am 'Add a new feature'`
 5. Push to the branch: `git push origin new-feature`
 6. Create a new Pull Request!

## Compiling

LiveSplit is written in C# 7 with Visual Studio and uses .NET Framework 4.6.1. To compile LiveSplit, you need one of these versions of Visual Studio:
 - Visual Studio 2017 Community Edition or later
 - Visual Studio 2017 or later

Simply open the project with Visual Studio and it should be able to compile and run without any further configuration.

## Common Compiling Issues
1. Could not build Codaxy.Xlio due to sgen.exe not being found. Open LiveSplit\\Libs\\xlio\\Source\\Codaxy.Xlio\\Codaxy.Xlio.csproj in order to edit where it looks for this path. Look for &lt;SGen...&gt; where it defines the attribute "ToolPath". Look on your computer to find the proper path. It is typically down some path such as "C:\\Program Files (x86)\\Microsoft SDKs\\Windows\\x.xA...". Find the version you want to use and bin folder with sgen.exe in it and replace the path in the .csproj file.
2. No submodules pulled in when you fork/clone the repo which causes the project not to build. There are two ways to remedy this:
 - Cloning for the first time: `git clone --recursive git://repo/repo.git`
 - If already cloned, execute this in the root directory: `git submodule update --init --recursive`

## Auto Splitters

The documentation for how to develop, test, and submit an Auto Splitter can be found here:

[Auto Splitters Documentation](https://github.com/LiveSplit/LiveSplit.AutoSplitters/blob/master/README.md)

## The LiveSplit Server

The internal LiveSplit Server allows for other programs and other computers to control LiveSplit. The server can accept connections over either a named pipe located at `\\<hostname>\pipe\LiveSplit` (`.` is the hostname if the client and server are on the same computer) or over TCP/IP.

### Control

The named pipe is always open while LiveSplit is running but the TCP server **MUST** be started before programs can talk to it (Right click on LiveSplit -> Control -> Start TCP Server). You **MUST** manually start it each time you launch LiveSplit.

### Settings

#### Server Port

**Server Port** is the door (one of thousands) on your computer that this program sends data through. Default is 16834. This should be fine for most people, but depending on network configurations, some ports may be blocked. See also https://en.wikipedia.org/wiki/Port_%28computer_networking%29

### Known Uses

- **Android LiveSplit Remote**: https://github.com/Ekelbatzen/LiveSplit.Remote.Android
- **SplitNotes**: https://github.com/joelnir/SplitNotes
- **Autosplitter Remote Client**: https://github.com/RavenX8/LiveSplit.Server.Client

Made something cool? Consider getting it added to this list.

### Commands

Commands are case sensitive and end with a carriage return and a line feed (\r\n). You can provide parameters by using a space after the command and sending the parameters afterwards (`<command><space><parameters><\r\n>`).

A command can respond with a message. The message ends with a carriage return and a line feed, just like a command.

Here's the list of commands:

- startorsplit
- split
- unsplit
- skipsplit
- pause
- resume
- reset
- starttimer
- setgametime TIME
- setloadingtimes TIME
- pausegametime
- unpausegametime
- alwayspausegametime
- setcomparison COMPARISON
- switchto realtime
- switchto gametime
- setsplitname INDEX NAME
- setcurrentsplitname NAME

The following commands respond with a time:

- getdelta
- getdelta COMPARISON
- getlastsplittime
- getcomparisonsplittime
- getcurrentrealtime
- getcurrentgametime
- getcurrenttime
- getfinaltime
- getfinaltime COMPARISON
- getpredictedtime COMPARISON
- getbestpossibletime

Other commands:

- getsplitindex
- getcurrentsplitname
- getprevioussplitname
- getcurrenttimerphase

Commands are defined at `ProcessMessage` in "CommandServer.cs".

### Example Clients

#### Python

```python
import socket

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.connect(("localhost", 16834))
s.send(b"starttimer\r\n")
```

#### Java 7+

```java
import java.io.IOException;
import java.io.PrintWriter;
import java.net.Socket;

public class MainTest {
    public static void main(String[] args) throws IOException {
        Socket socket = new Socket("localhost", 16834);
        PrintWriter writer = new PrintWriter(socket.getOutputStream());
        writer.write("starttimer\r\n");
        writer.flush();
        socket.close();
    }
}
```

#### Node.js

Node.js client implementation available here: https://github.com/satanch/node-livesplit-client

## Releasing

1. Update versions of any components that changed (create a Git tag and update the factory file for each component) to match the new LiveSplit version.
2. Create a Git tag for the new version.
3. Download `LiveSplit_Build` from the GitHub Actions build for the new Git tag.
4. Create a GitHub release for the new version, and upload the LiveSplit build ZIP file with the correct filename (e.g. `LiveSplit_1.8.21.zip`).
    - Create a release for [`LiveSplit.Counter`](https://github.com/LiveSplit/LiveSplit.Counter) if necessary.
5. Modify files in [the update folder of LiveSplit.github.io](https://github.com/LiveSplit/LiveSplit.github.io/tree/master/update) and commit the changes:
    - Copy changed files from the downloaded LiveSplit build ZIP file to the update folder.
    - Add new versions to the update XMLs for (`update.xml`, `update.updater.xml`, and the update XMLs for any components that changed).
    - Update the version on the [downloads page](https://github.com/LiveSplit/LiveSplit.github.io/blob/master/downloads.md).

## License

The MIT License (MIT)

Copyright (c) 2013 Christopher Serr and Sergey Papushin

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
