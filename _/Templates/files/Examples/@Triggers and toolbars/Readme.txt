In files of this folder you can define all your triggers (hotkeys etc) and toolbars.
There are several examples. Click the Run button to run the script and see how it works.
Add your triggers near the example triggers. Delete or comment out unused examples.
Add/remove toolbars in a similar way. Create more "partial" files for toolbars if need.
After editing project files, click the Run button to restart the script.
To run the script at program startup, add its name in Options -> General -> Run scripts...

This folder is a script project. It is one big script divided into multiple files. The
script starts in file "Triggers and toolbars". It calls functions defined in other files.
Then it runs all the time, waiting for trigger events and clicked toolbar buttons.
When you press a defined hotkey or click a toolbar button etc, it executes the trigger
 action or button action (code that follows the =>).

If the action code is big, you can place it in a seperate function or script. See examples.
If the code is in a separate function, it starts faster and can share variables (fields) with
other functions of the project. The function can be anywhere in the project folder.
If the code is in a separate script, it runs in separate process and therefore starts slower
and does not share variables. But then it's easier to edit and test it, because don't need
to restart the main script. And it cannot crash the main process (triggers and toolbars).
The script can be anywhere (in project folder or outside).

To add a trigger or toolbar or button, you can use snippets. Start typing "trig" or "tool",
and you'll see snippets in the completion list.

When you add a new toolbar, also need to call the toolbar function, like in the examples.
When deleting a toolbar, also remove the function call.
To disable a toolbar temporarily, just comment out the function call.

To disable a trigger, comment out the trigger code line or move it to another script. Then
click Run to restart the script. To disable/enable all triggers, use the tray icon (menu
or middle-click) or menu Run. See also examples in class ActionTriggers help.
To disable all triggers of a type (eg all hotkeys), comment out the [Triggers] attribute.

Avoid multiple scripts with triggers running all the time. It makes your computer slower.

Also you can define menus. They are like toolbars, but displayed temporarily and therefore
can be in any script. To create menus and menu items, use snippets that start with "menu".
