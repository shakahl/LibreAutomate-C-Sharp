\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_draw_control" &dlg_draw_control)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030203 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_PAINT
	
	PAINTSTRUCT ps; BeginPaint hDlg &ps
	
	 RECT rc; GetClientRect(hDlg &rc)
	 FillRect ps.hDC &rc COLOR_BTNFACE+1
	
	 RECT r.right=20; r.bottom=20
	 DrawFrameControl(ps.hDC &r DFC_BUTTON DFCS_BUTTONCHECK|DFCS_FLAT|DFCS_CHECKED)
	
	int theme=OpenThemeData(0 L"Button")
	out theme
	
	RECT r rCont
	r.right=100; r.bottom=30
	out DrawThemeBackground(theme ps.hDC WINAPI2.BP_CHECKBOX WINAPI2.CBS_UNCHECKEDNORMAL &r 0)
	
	CloseThemeData theme
	
	EndPaint hDlg &ps
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
