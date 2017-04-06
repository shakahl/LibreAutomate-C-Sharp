\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 4"
str e3 c4Che
if(!ShowDialog("dlg_hidden_modal" &dlg_hidden_modal &controls 0 128)) ret
 if(!ShowDialog("dlg_hidden_modal" &dlg_hidden_modal &controls 0 128)) ret
 if(!ShowDialog("dlg_hidden_modal" &dlg_hidden_modal &controls win 128)) ret
 if(!ShowDialogHidden("dlg_hidden_modal" &dlg_hidden_modal &controls)) ret
out e3

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog Hidden"
 3 Edit 0x54030080 0x200 6 8 96 14 ""
 4 Button 0x54012003 0x0 6 28 48 13 "Check"
 1 Button 0x54030001 0x0 118 116 48 14 "OK"
 2 Button 0x54030000 0x0 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "*" "" ""


 if(!ShowDialog("dlg_hidden_modal" &dlg_hidden_modal 0 0 128)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "*" "" ""

ret
 messages
 OutWinMsg message wParam lParam
sel message
	case WM_INITDIALOG
	SetTimer hDlg 1 2000 0
	out
	
	case WM_COMMAND goto messages2
	case WM_DESTROY
	 out "destr"
	
	case WM_TIMER
	sel wParam
		 case 1 clo hDlg
		  case 1 DestroyWindow hDlg
		case 1
		KillTimer hDlg wParam
		 act hDlg
		hid- hDlg
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
	 ret
ret 1

#ret
