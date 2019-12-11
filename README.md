# Au automation library and editor

Au library contains functions to automate various tasks on Windows computers. Written mostly in C#, it is a .NET library. Some features: Send keys and text to the active window. Find and click buttons, links, images and other UI objects. Launch programs and auto-close windows. Process text and other data. Auto-replace text when typing. Hotkeys and other triggers.

Au editor program is an integrated scripting environment for creating and executing automation scripts using the Au library and C#.

More info and download the setup file: https://www.quickmacros.com/au/help/

Editor window

![window](https://www.quickmacros.com/au/help/images/window.png "Editor window")

### How to build
Need Visual Studio 2019 with C# 8, C++, .NET Core 3.1 and Windows 10 SDK.

1. Open Au.sln in Visual Studio. Ignore "failed to load project" errors.
2. Build solution (not just the startup project).
3. Switch to platform x86, build solution, switch back to AnyCPU.
4. Run _Au.Editor project. It should open the editor window.
