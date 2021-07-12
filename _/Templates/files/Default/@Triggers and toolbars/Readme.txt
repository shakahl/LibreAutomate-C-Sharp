In files of this folder you can define triggers (hotkeys etc) and toolbars.
There are several examples. Click the Run button to run the script and see how it works.
Add your triggers near the example triggers. Delete or comment out unused examples.
Add/remove toolbars in a similar way. Create more "partial" files for toolbars if need.
After editing project files, click the Run button to restart the script.

This folder and its missing files are automatically added when you click a TT menu item.
Also it sets to run the script at program startup (Options -> General -> Run scripts...).

This folder is a script project. It is one script divided into multiple files. The
script starts in file "Triggers and toolbars". It calls function RunTriggersAndToolbars,
which calls functions with attribute [Triggers] or [Toolbars]; they are in other files.
Then it runs all the time, waiting for trigger events and clicked toolbar buttons.
When you press a defined hotkey or click a toolbar button etc, it executes the trigger
action or button action (code that follows the =>).

Add triggers in functions with attribute [Triggers]. Toolbars - with attribute [Toolbars].
To add a trigger or toolbar or button, you can use snippets. Start typing "trig" or "tool",
and you'll see snippets in the completion list.

When you add a new toolbar, also need to call the toolbar function. Usually for it you
add a trigger. When deleting a toolbar, also remove the function call or trigger.
To disable a toolbar temporarily, just comment out the function call or trigger.

When trigger action code is big, you can place it in a seperate script. See examples.
Then it runs in separate process and therefore starts slower. You can edit and test the
script without restarting the main script process (triggers and toolbars). And it cannot
crash the main process. The script can be anywhere (in project folder or outside).

To disable a trigger, comment out the trigger code line or move it from this folder. Then
click Run to restart the script. To disable/enable all triggers, use the tray icon (menu
or middle-click) or menu TT. See also examples in class ActionTriggers help.
Also you can comment out the [Triggers] attribute to disable all triggers defined there.

Avoid multiple scripts with triggers running all the time. It makes your computer slower.

Also you can define menus. They are like toolbars, but displayed temporarily and therefore
can be in any script. To create menus and menu items, use snippets that start with "menu".

If after editing or deleting files something does not work and you don't know how to fix it:
1. Click a TT menu item Edit... . It adds missing files.
	If you want to replace an existing file, at first remove it from the folder.
2. Or create new project again: menu File -> New -> Default -> @Triggers and toolbars.
	Then either fix your old project or move triggers etc from it to the new project.
	Then delete the unused project folder and restore folder name if changed.
