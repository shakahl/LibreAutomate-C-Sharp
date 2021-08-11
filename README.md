# Derobotize Me C#

Derobotize Me C# is a free automation library and advanced C# script editor/manager/compiler/launcher (ISE). In scripts you can use keys, mouse, window/webpage elements, files, internet, text processing, dialog windows, floating toolbars, hotkey/autotext/etc triggers, Windows API, .NET and zillion of programming libraries, as well as code examples and info from the internet. Can create programs and libraries, and not just for automation. You'll gradually learn main automation functions and C#, one of top 5 programming languages. Then creating automations with this code editor is faster than with no-code RPA apps, and less limitations.

The library contains functions to automate various tasks on Windows computers. Written mostly in C#, it is a .NET library. Some features: Send keys and text to the active window. Find and click buttons, links, images and other UI objects. Launch programs and auto-close windows. Process text and other data. Auto-replace text when typing. Hotkeys and other triggers.

The editor program is an integrated scripting environment for creating and executing automation scripts using the library and C#.

More info and download the setup file: https://www.quickmacros.com/au/help/

Editor window

![window](https://www.quickmacros.com/au/help/images/window.png "Editor window")

### How to build
Need Visual Studio 2019 with C#, C++, .NET 5.0 SDK and Windows 10 SDK. If VS still does not support C# 10, need VS preview.

1. Open Au.sln in Visual Studio. Ignore "failed to load project" errors.
2. Build solution (not just the startup project).
3. Switch to platform x86, build solution, switch back to AnyCPU.
4. Copy missing files (.db, .dll, etc) from the installed program folder to the output folder "_".
5. Run Au.Editor project. It should open the editor window.
