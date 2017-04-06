\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("Dialog25" &Dialog25)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 221 133 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 0 119 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2020001 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 3
	0
	Sleep 10000
	 SleepEx 10000 1
	 rep 200000000
		 _i=8
	case IDOK
	case IDCANCEL
ret 1
