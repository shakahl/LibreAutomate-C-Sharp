\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 4"
str e3 e4
if(!ShowDialog("dialog_color_focus_rect" &dialog_color_focus_rect &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 Edit 0x54030080 0x200 8 14 56 14 ""
 4 Edit 0x54030080 0x200 110 14 96 14 ""
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030508 "*" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_DRAWITEM
	DT_SCFC_on_WM_DRAWITEM hDlg wParam lParam
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case [EN_SETFOCUS<<16|3,EN_SETFOCUS<<16|4]
	DT_ShowColorFocusControl hDlg
	
	case IDOK
	case IDCANCEL
ret 1
