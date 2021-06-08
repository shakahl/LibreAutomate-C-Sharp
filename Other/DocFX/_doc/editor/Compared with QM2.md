---
uid: qm2
---

# Compared with Quick Macros 2
This program is the new major version of Quick Macros 2 (QM). Let's call it QM3. It has or will have most of QM features. But it is very different.

The script language now is C#. It is one of most popular programming languages and has many libraries.

QM3 cannot execute or convert QM scripts. If you have many active QM scripts, probably not worth to convert them. Let both programs run at the same time.

QM3 is free and open-source. Its open-source automation library can be used in other programs too.

QM3 can run on Windows 7-10 with .NET runtime 5. More info [here](xref:index).

QM3 and its library are still unfinished. The most important parts are finished and can be used, but some classes and functions can be changed in the future, which can break your scripts, clear settings, etc.

QM (Quick Macros 2) will not have more features in the future. Only bug fixes. But it will be available and supported for long time.

Let's compare QM and QM3 code.

A script in QM:
```csharp
int w=win("- Mozilla Firefox" "MozillaWindowClass")
act w
lef 331 115 w 1
2
key L "text" Y
int i
for i 0 5
	out i
	out i*i
```

The same script in QM3 (C#):
```csharp
var w = wnd.find("*- Mozilla Firefox", "MozillaWindowClass");
w.Activate();
mouse.click(w, 331, 115);
2.s();
keys.send("Left", "text", "Enter");
for(int i=0; i<5; i++) {
	print.it(i);
	print.it(i*i);
}
```

As you see, C# code is longer, but usually it is easier to understand. The [code editor](xref:code_editor) can add `; () { }` automatically as you type, and has other code editing features that are much better than in QM.

QM3 has triggers to execute parts of a running script. Trigger types: hotkey, autotext, mouse, window; more in the future. Triggers also can be used to launch scripts, but differently than in QM.

QM3 does not have item types like "menu", "toolbar" and "autotext". Instead use classes popupMenu, toolbar and AutotextTriggers.

Instead of dialogs now use Windows Forms or WPF. Editor in the future.

Currently QM3 has only the most important code creation tools in the Code menu: keys, regex, find window/control/accessible/image, Windows API. More in the future.

In QM3 each script is a separate file. [Read more](xref:au_editor).

In QM3 each script runs in a separate process.

In QM3 each script can use only namespaces it wants to use. Namespaces contain classes; classes contain functions. In QM all scripts share all classes and global functions.

QM3 can create .NET class libraries (.dll files).

In QM3 window you can resize and dock all panels and toolbars where you want, or make floating.

QM3 saves all settings in files, not in the Registry.

#### Some important features still missing
- Recording.
- Dialog editor.
- Most Tools menu commands.
- Many options in the Options dialog.
- Automatic backup.
- Triggers of types other than hotkey, autotext, mouse and window.
- UI to create Windows Task Scheduler tasks easier.
- Debugger. To debug a script in step mode now need Visual Studio.
- Multiple visible code editor controls.

Most of these will be added in the future.

#### Some features QM3 will never have
- Encrypt scripts.
- Unlock computer. Maybe in distant future.

#### Portable
QM3 should be able to run as a portable app, but it is not tested and there is no tool to create a portable app folder like in QM. Copy all files from the installed QM3 folder and run Aedit.exe. Portable QM3 does not modify the Registry, but writes files in folders like Temp and Documents. Runs not as administrator and therefore cannot automate windows of admin processes without invoking the UAC consent screen.
