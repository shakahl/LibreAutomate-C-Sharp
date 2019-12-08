# Au automation library and editor

This code consists of two parts that can be used together or separately:
- Au library is a .NET library for automating of various tasks on Windows computers. Send keys to windows, find/click UI objects, execute programs, process text, use hotkeys, autotext.
- Au editor program is an integrated scripting environment for creating and executing automation scripts using the Au library and C#. And creating .NET programs and libraries for any purpose. Like a mini Visual Studio.

More info and download the setup program: https://www.quickmacros.com/au/help/

Editor window

![window](https://www.quickmacros.com/au/help/images/window.png "Editor window")

### How to build
Need Visual Studio 2019. It must support C# 8 and .NET Core 3.0.

1. Open Au.sln in Visual Studio. Ignore "failed to load project" errors.
2. Build solution (not just the startup project).
3. Switch to platform x86, build solution, switch back to AnyCPU.
4. Run _Au.Editor project. It should open the editor window.
