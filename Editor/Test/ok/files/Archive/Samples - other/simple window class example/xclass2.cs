 register class
int+ __xclass
if(!__xclass) __xclass=RegWinClass("xclass" &WndProc_xclass)

 create window
int style=WS_POPUP|WS_BORDER
int exstyle=WS_EX_TOOLWINDOW
int hwnd=CreateWindowEx(exstyle "xclass" 0 style 10 10 50 50 0 0 _hinst 0)

OnScreenDisplay "do you see your window in the top-left corner?"

 wait, or show a dialog

if(!ShowDialog("xclass2" 0)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030006 "*" "" ""


 destroy
DestroyWindow hwnd
