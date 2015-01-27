[![Stories in Ready](https://badge.waffle.io/LiveSplit/LiveSplit.png?label=ready&title=Ready)](https://waffle.io/LiveSplit/LiveSplit)
LiveSplit
=========

A sleek, highly-customizable timer for speedrunners.

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
 - Visual Studio 2015 Preview

Preferably you should either get a full version of Visual Studio 2013 or the Community Edition if you don't want to pay for Visual Studio.

Simply open the project with Visual Studio and it should be able to compile and run it without any further configuration.

## Adding an Auto Splitter

If you implemented an Auto Splitter and want to add it to the Auto Splitters that are automatically being downloaded by LiveSplit, feel free to add it to the [Auto Splitters XML](https://github.com/LiveSplit/LiveSplit/blob/master/LiveSplit.AutoSplitters.xml). Just click the link, click the icon for modifying the file and Github will automatically create a fork, branch and pull request for you, which we can review and then merge in.

## License

The MIT License (MIT)

Copyright (c) 2014 Christopher Serr and Sergey Papushin

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
