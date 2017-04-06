 Manages settings that you use in a macro.
 Saves settings in registry. Displays in grid control, where user can change them.
 Stores settings as a list of name/value pairs. Macro can get values by name. Values can have multiple columns.

 At first call Init. With it you define names, default values, grid control row labels, and registry key/name.
 The csv argument used with Init is a CSV table. Later you can change it: add/remove/reorder rows; add/remove columns (only to/from the end); cannot reorder columns.

 Then call FromReg or FromString. It replaces default values with changed/saved values. Don't need it if you use flag 1 with Init.
 Then you can use the settings. Call GetStr, GetInt, GetCheck.

 To show simple dialog where user can change settings, call OptionsDialog.
 If you need custom dialog:
   Create dialog. Add Grid control.
   To display settings in Grid control, call ToGridVar (before ShowDialog) or ToGrid (in dialog procedure).
   To get settings from Grid control, call FromGridVar (after ShowDialog) or FromGrid (in dialog procedure). Then call ToReg or ToString to save.
 See sample dialogs: <open>dlg_test___Settings_simple</open>, <open>dlg_test___Settings</open>.

 The functions don't throw errors, unless some argument is incorrect.

 This class is not thread-safe. If you use same settings in multiple threads (running macros), each thread must have its own variable of this class (local or thread scope, not global). You can use global variables for Init arguments.

 EXAMPLE
__Settings x.Init("name,Name,me[]speed,Speed,100" "settings" "\MyCompany\MyMacro" 1)
out "%s %i" x.GetStr("name") x.GetInt("speed")

if(!x.OptionsDialog(0 "MyMacro settings")) ret
out "%s %i" x.GetStr("name") x.GetInt("speed")
