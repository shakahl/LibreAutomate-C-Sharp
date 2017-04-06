\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 4"
str e3 e4
if(!ShowDialog("dlg_edit_color" &dlg_edit_color &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Edit 0x54030080 0x200 8 10 96 14 ""
 4 Edit 0x54030080 0x200 8 32 96 14 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030001 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
	
	case WM_CTLCOLOREDIT
	 wParam is handle to the device context of the control
	 lParam is control handle
	
	GdiObject-- brush1 brush2
	if(!brush1) brush1=CreateSolidBrush(ColorFromRGB(240 240 100))
	if(!brush2) brush2=CreateSolidBrush(ColorFromRGB(128 128 255))
	
	sel(GetWinId(lParam))
		case 3
		SetBkMode wParam TRANSPARENT
		SetTextColor wParam ColorFromRGB(0 0 255)
		ret brush1
		case 4
		SetBkMode wParam TRANSPARENT
		SetTextColor wParam ColorFromRGB(255 0 0)
		ret brush2
	
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
