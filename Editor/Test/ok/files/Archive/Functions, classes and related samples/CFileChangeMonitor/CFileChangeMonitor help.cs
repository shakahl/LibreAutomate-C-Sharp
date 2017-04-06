 Registers a window to receive notifications about changes in a folder.
 Eg when a file added/removed/modified/renamed or changed properties.
 OS and apps generate the notifications with SHChangeNotify. It is documented in MSDN library.
 Uses SHChangeNotifyRegister. It is not well documented, and possibly not reliable.
 Little tested.
 Use when reliability is not very important.
 For more info, google for SHChangeNotifyRegister.

 <open "dlg_shell_notifications">Example</open>
