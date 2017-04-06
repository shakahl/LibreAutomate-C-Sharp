\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_test_monitor" &dlg_test_monitor 0)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 6 6 48 14 "mes"
 4 Button 0x54032000 0x0 58 6 48 14 "inp"
 5 Button 0x54032000 0x0 110 6 48 14 "list"
 6 Button 0x54032000 0x0 162 6 48 14 "ShowDialog"
 7 Button 0x54032000 0x0 6 26 48 14 "OSD"
 8 Button 0x54032000 0x0 58 26 48 14 "menu (does not work)"
 9 Button 0x54032000 0x0 110 26 48 14 "EWIS"
 10 Button 0x54032000 0x0 162 26 48 14 "MWTM"
 11 Button 0x54032000 0x0 6 46 48 14 "Curtain"
 END DIALOG
 DIALOG EDITOR: "" 0x2030002 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	 _monitor=2
	_monitor=hDlg
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 3 mes 1
	case 4 inp _s
	case 5 list "q"
	case 6
	str dd=
 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog2"
 END DIALOG
 DIALOG EDITOR: "" 0x2030002 "*" "" ""
	ShowDialog(dd 0 0 0 2)
	case 7 OnScreenDisplay "aaaa"
	case 8 DynamicMenu " /pos center[]a[]b"
	case 9 EnsureWindowInScreen win("Dialog2")
	
	case 10
	int h=win("Dialog2")
	 act h
	MoveWindowToMonitor h 0
	
	case 11
	Curtain
	
	case IDOK
	case IDCANCEL
ret 1
