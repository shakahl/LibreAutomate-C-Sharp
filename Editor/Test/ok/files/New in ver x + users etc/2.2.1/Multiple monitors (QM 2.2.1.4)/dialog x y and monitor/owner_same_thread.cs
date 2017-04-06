\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 _monitor=2

if(!ShowDialog("" &owner_same_thread)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 12 14 48 14 "default"
 4 Button 0x54032000 0x0 68 14 48 14 "x y"
 5 Button 0x54032000 0x0 124 14 48 14 "unowned"
 6 Button 0x54032000 0x0 12 38 48 14 "OSD"
 END DIALOG
 DIALOG EDITOR: "" 0x2020104 "" "" ""

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
	 _monitor=2
	if(!ShowDialog("owned_same_thread" 0 0 hDlg)) ret

	case 4
	 _monitor=2
	if(!ShowDialog("owned_same_thread" 0 0 hDlg 0 0 0 0 100 -30)) ret

	case 5
	 _monitor=0
	if(!ShowDialog("owned_same_thread")) ret

	case 6
	 _monitor=0
	OnScreenDisplay "Text 1WWWWW 2WWWWW 3WWWWW"

	case IDOK
	case IDCANCEL
ret 1

