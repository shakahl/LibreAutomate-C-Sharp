\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Static 0x54030100 0x4 16 84 48 13 "&ClickHere"
 4 Edit 0x54230844 0x20000 0 94 96 48 "dddd"
 END DIALOG
 DIALOG EDITOR: "" 0x2020100 "" ""
if(!ShowDialog("dlg_highlight_controls" &dlg_highlight_controls)) ret
ret
 messages
int z
z=DT_HighlightControlOnMouseOver(hDlg message wParam lParam id(3 hDlg) 0xff0000 0x00ffff)
if(z) ret z
z=DT_HighlightControlOnMouseOver(hDlg message wParam lParam id(4 hDlg) 0xff0000 0x00ffff)
if(z) ret z
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case STN_CLICKED<<16|3
	out 1
	case IDOK
	case IDCANCEL
ret 1
