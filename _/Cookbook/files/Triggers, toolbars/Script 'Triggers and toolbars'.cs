/// It is recommended to keep toolbars and keyboard/autotext/mouse/window triggers in folder "@Triggers and toolbars". It contains script "Triggers and toolbars", split into multiple files. Click a TT menu item to open a file; it creates the folder/file if need.
/// 
/// Each file in the folder contain several examples, initially disabled. You can enable them and run the script to see how it works.
/// Add your triggers near the example triggers. Delete or comment out unused examples.
/// To add toolbars, use menu TT -> New toolbar.
///
/// Triggers and toolbars are active only when the script is running. When the program creates the folder, it also sets to run the script at program startup (Options -> General -> Run scripts...). Need to restart it (click the Run button) to apply changes after editing triggers or toolbars.
/// 
/// How it works: When the script starts, it calls functions with attribute [Triggers] or [Toolbars]. They are in other files; they contain triggers and toolbars (you added them there). Then the script runs all the time, waiting for trigger/toolbar events (hotkey, button click, etc). On event it executes the trigger/button action (code that follows the =>).
/// 
/// When trigger action code is big, you can place it in a seperate script. See examples. Then it runs in separate process and therefore starts slower. You can edit and test the script without restarting the main script process (triggers and toolbars). And it cannot crash the main process. The script can be anywhere (in the folder or outside).
/// 
/// To disable a trigger, comment out the trigger code line. Then click Run to restart. To disable/enable all triggers, use the tray icon (menu or middle-click) or menu TT. Also you can comment out the [Triggers] attribute to disable all triggers defined there. See also class <help Au.Triggers.ActionTriggers>ActionTriggers<> examples.
/// 
/// Avoid multiple scripts with triggers running all the time. Each script uses some CPU, which makes the computer slightly slower. Particularly input events (key, mouse) and window events. That is why triggers should be in folder "@Triggers and toolbars".
/// 
/// Also you can create <+recipe Popup menu>menus<>. They are like toolbars, but displayed temporarily and therefore can be in any script.
/// 
/// If after editing or deleting files something does not work and you don't know how to fix it:
/// - Click a TT menu item Edit... . It adds missing files. If you want to replace an existing file, at first remove it from the folder.
/// - Or create new folder again: menu File -> New -> Default -> @Triggers and toolbars. Then either fix your old folder or move triggers etc from it to the new folder. Then delete the unused folder and restore folder name if changed.
