\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "4 7 3 5"
str e4 e7 c3 c5
if(!ShowDialog("dlg_rod_berlin" &dlg_rod_berlin &controls)) ret

 BEGIN DIALOG
 0 "" 0x10C80A44 0x100 0 0 219 131 "Form"
 4 Edit 0x540310C4 0x204 10 38 38 12 ""
 7 Edit 0x54030080 0x204 10 56 38 12 ""
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54012003 0x4 8 72 210 12 "90801 -- Check This Box if Billing 90801"
 5 Button 0x54012003 0x4 8 88 210 10 "90862 -- Check This Box if Billing 90862"
 6 Static 0x54020000 0x4 52 40 58 10 "Enter Allowable"
 8 Static 0x54020000 0x4 52 58 120 10 "Enter Payment Amount Received"
 END DIALOG
 DIALOG EDITOR: "" 0x2010900 "" ""

ret
 messages
sel message
	case WM_INITDIALOG DT_Init(hDlg lParam); ret 1
	case WM_DESTROY DT_DeleteData(hDlg)
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case EN_CHANGE<<16|4
	_s.getwintext(lParam); if(_s.end("[]")) key BT
	case EN_SETFOCUS<<16|7
	case IDOK DT_Ok hDlg
	case IDCANCEL DT_Cancel hDlg
ret 1