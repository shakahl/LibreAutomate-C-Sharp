
str dd=
 BEGIN DIALOG
 0 "" 0x90CB0AC8 0x0 0 0 224 136 "Dialog MM"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040400 "*" "" "" ""

 0 "" 0x10CF0AC8 0x0 0 0 224 136 "Dialog MM"

_monitor=2
if(!ShowDialog(dd &sub.DlgProc 0)) ret


#sub DlgProc
function# hDlg message wParam lParam

 OutWinMsg message wParam lParam

sel message
	case WM_INITDIALOG
	case WM_WINDOWPOSCHANGING
	WINDOWPOS& r=+lParam
	out F"0x{r.flags} {r.x} {r.y} {r.cx} {r.cy}"
	 r.flags|SWP_NOMOVE|SWP_NOSIZE|SWP_NOACTIVATE|SWP_SHOWWINDOW ;;ignored when minimizing (minimizes anyway)
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

 BEGIN PROJECT
 main_function  Dialog205
 exe_file  $my qm$\Dialog205.qmm
 flags  6
 guid  {BA717D24-86E1-4649-9AB2-1DD4E80688D2}
 END PROJECT
