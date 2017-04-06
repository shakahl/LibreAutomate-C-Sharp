 Adds tray icon and stays running.
 Sub-function TrayProc is called on mouse events.

if(getopt(nthreads)>1) ret
Tray t.AddIcon("notepad.exe" "QM - run Notepad" 0 &sub.TrayProc)
MessageLoop


#sub TrayProc
function Tray&tray msg

sel msg
	case WM_LBUTTONUP
	sel GetMod
		case 0 run "notepad.exe"
		case 2 shutdown -7
	
	case WM_RBUTTONUP
	sel ShowMenu("1 Run notepad[]2 Exit")
		case 1 run "notepad.exe"
		case 2 shutdown -7
