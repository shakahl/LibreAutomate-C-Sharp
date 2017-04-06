 Adds two menu items to the system menu of Notepad.

int hwnd=win("" "Notepad")
int hmenu=GetSystemMenu(hwnd 0)
if(!hmenu) ret
AppendMenu hmenu MF_DISABLED 0xA001 "test"
AppendMenu hmenu MF_DISABLED 0xA002 "test2"
SetProp hwnd "symt_hmenu" hmenu
