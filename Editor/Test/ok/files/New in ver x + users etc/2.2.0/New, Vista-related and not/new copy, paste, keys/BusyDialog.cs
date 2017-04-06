\Dialog_Editor

 Don't use this for key testing. Use BusyDialog2 instead. Use this for act testing.

function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str e3
if(!ShowDialog("" &BusyDialog &controls 0 0 0 0 0 1 1)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 221 215 "Form"
 3 Edit 0x54231044 0x200 0 0 222 196 ""
 1 Button 0x54030001 0x4 122 198 48 14 "OK"
 2 Button 0x54030000 0x4 172 198 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2020001 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	SetTimer hDlg 1 1800 0
	case WM_TIMER
	Sleep 1700
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
