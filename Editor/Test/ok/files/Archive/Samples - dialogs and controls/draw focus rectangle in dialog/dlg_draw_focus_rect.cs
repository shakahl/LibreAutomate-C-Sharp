\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str rea3
if(!ShowDialog("dlg_draw_focus_rect" &dlg_draw_focus_rect &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 RichEdit20A 0x54233044 0x200 20 20 96 48 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030006 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
	
	case WM_SETCURSOR
	 if(DragDrawFocusRect(hDlg wParam lParam)) ret DT_Ret(hDlg 1)
	if(DragDrawFocusRect(hDlg wParam lParam 3)) ret DT_Ret(hDlg 1)
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
