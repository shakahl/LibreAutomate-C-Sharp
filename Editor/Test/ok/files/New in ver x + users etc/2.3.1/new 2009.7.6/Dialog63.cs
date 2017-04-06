\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str si3
if(!ShowDialog("Dialog63" &Dialog63 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Static 0x54000103 0x0 0 0 16 16 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030100 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	 when dialog starts, load 2 icons
	ARRAY(__Hicon)-- t_ai; int-- t_aii ;;array of icons and current icon index
	t_ai.create(2)
	t_ai[0]=GetFileIcon("shell32.dll" 8 1)
	t_ai[1]=GetFileIcon("shell32.dll" 9 1)
	SendMessage id(3 hDlg) STM_SETICON t_ai[0] 0
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_SETCURSOR
	 detect mouse over the static icon control (it must have SS_NOTIFY style)
	if(GetDlgCtrlID(wParam)=3)
		if(t_aii=0)
			t_aii=1
			SendMessage id(3 hDlg) STM_SETICON t_ai[1] 0
			SetTimer hDlg 100 50 0
	case WM_TIMER
	 detect when mouse leaves the control
	sel wParam
		case 100
		if(child(mouse)!=id(3 hDlg))
			KillTimer hDlg wParam
			t_aii=0
			SendMessage id(3 hDlg) STM_SETICON t_ai[0] 0
			
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
