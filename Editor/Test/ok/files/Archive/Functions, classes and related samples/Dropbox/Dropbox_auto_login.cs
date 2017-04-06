 Automatically enters email and password in the "Dropbox" window shown by Dropbox.Authorize().
 In this macro change "email" and "password", and enable this macro.

int hwnd=TriggerWindow
 outw hwnd
AutoPassword "email" "password" 1|4 hwnd 1
