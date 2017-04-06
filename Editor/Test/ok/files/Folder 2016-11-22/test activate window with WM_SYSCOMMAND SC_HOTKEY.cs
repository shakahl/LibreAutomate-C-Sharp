 /exe 1
int w=win("Untitled - Notepad" "Notepad")
2
 out SetForegroundWindow(w); ret
 SendMessage w WM_SYSCOMMAND SC_HOTKEY w; ret ;;does not work

 int wman=win("{115A99F4-4C6E-43EC-A238-2C145629BE81}" "QM_ExeManager")
int wman=CreateWindowEx(0 "#32770" 0 WS_POPUP 0 0 0 0 0 0 0 0)
 outw wman
if(SendMessage(wman WM_SETHOTKEY MakeInt(VK_F6 HOTKEYF_CONTROL|HOTKEYF_SHIFT|HOTKEYF_ALT) 0) != 1) end "failed"
 SendMessage wman WM_SYSCOMMAND SC_HOTKEY wman ;;does not work
opt keysync 1
key CSAF6 ;;yes
DestroyWindow wman

rep 3
	out SetForegroundWindow(w)
	2

 BEGIN PROJECT
 flags  6
 guid  {115A99F4-4C6E-43EC-A238-2C145629BE81}
 END PROJECT
