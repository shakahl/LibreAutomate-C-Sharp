\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

dll "qm.exe" #WaitMsg2 ms [!forwardmessages] [*hp] [nh] [!all]

str controls = "4"
str e4
if(!ShowDialog("Dialog50" &Dialog50 &controls _hwndqm)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog50"
 4 Edit 0x54030080 0x200 20 10 96 14 ""
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 34 30 48 14 "WaitMsg 5"
 END DIALOG
 DIALOG EDITOR: "" 0x203000E "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	__Handle- h=CreateEvent(0 0 0 0)
	 int h=87654321
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_APP+4
	SetEvent h
	 CloseHandle h; h=0
	ret DT_Ret(hDlg wParam*2)
	case WM_TIMER
	sel wParam
		case 1
		KillTimer hDlg wParam
		SetEvent h
ret
 messages2
sel wParam
	case 3
	SetFocus id(4 hDlg)
	 PostQuitMessage 0
	SetTimer hDlg 1 1000 0
	 out WaitMsg2(5000 0 &h 1)
	 out WaitMsg2(5000 1 &h 1)
	 out WaitMsg2(0 0 &h 1)
	 out WaitMsg2(0 1 &h 1)
	 out WaitMsg2(0)
	 out WaitMsg2(0 1)
	out WaitMsg2(1000)
	case IDOK
	case IDCANCEL
ret 1
