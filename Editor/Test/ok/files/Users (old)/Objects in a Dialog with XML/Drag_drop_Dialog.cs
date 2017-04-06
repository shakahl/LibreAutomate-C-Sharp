\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str e3
if(!ShowDialog("Drag_drop_Dialog" &Drag_drop_Dialog &controls)) ret

 BEGIN DIALOG
 0 "" 0x90CC0A44 0x100 0 0 222 134 "Dialog"
 4 Static 0x54000000 0x0 144 2 76 22 "Move with Shift[]Resize with Ctrl"
 3 Button 0x54032000 0x0 2 2 48 14 "Add"
 5 Button 0x54032000 0x0 52 2 48 14 "Delete"
 END DIALOG
 DIALOG EDITOR: "" 0x2030001 "" "" ""

ret
 messages
int-- dirty hfocus
sel message
	case WM_INITDIALOG
	XML_Load hDlg 1
	case WM_DESTROY
	if(dirty) XML_Save hDlg
	case WM_COMMAND goto messages2
	
	case WM_SETCURSOR ;;use it to detect left button down. Don't use WM_LBUTTONDOWN because it is sent to the control, not to the dialog.
	sel lParam>>16 ;;mouse message
		case WM_LBUTTONDOWN ;;left down
		if GetDlgCtrlID(wParam)>=100 ;;an edit control
			int m=GetMod
			sel m
				case [1,2] ;;Shift or Ctrl
				PostMessage hDlg WM_APP wParam m ;;don't use loop on WM_SETCURSOR. Do it async.
				ret DT_Ret(hDlg 1)
	
	case WM_APP
	MoveSizeControlLoop2 hDlg wParam lParam 1
	XML_Save hDlg
	
	case WM_TIMER
	sel wParam
		case 1 ;;save
		KillTimer hDlg 1
		if(dirty)
			XML_Save hDlg
			dirty=0
	
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
	case 3 ;;Add
	XML_AddControl hDlg
	case 5
	XML_DeleteControl hDlg hfocus
	hfocus=0

if(wParam&0xffff>=100) ;;an edit control
	sel wParam>>16
		case EN_CHANGE
		SetTimer hDlg 1 1000 0 ;;save after 1 s
		dirty=1
		
		case EN_SETFOCUS
		hfocus=lParam

ret 1
