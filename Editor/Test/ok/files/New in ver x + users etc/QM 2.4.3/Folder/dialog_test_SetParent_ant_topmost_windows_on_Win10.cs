\Dialog_Editor
out
str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog1"
 3 Edit 0x54030080 0x200 8 20 96 13 ""
 4 Button 0x54032000 0x0 8 4 48 14 "Button"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040202 "*" "" "" ""

str dd2=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x8 0 0 224 136 "Dialog2"
 3 Edit 0x54030080 0x200 8 50 96 13 ""
 4 Button 0x54032000 0x0 8 4 48 14 "Button"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040202 "*" "" "" ""

str controls = "3"
str e3
int w1=ShowDialog(dd &sub.DlgProc &controls _hwndqm 1 0 0 0 400)
int w2=ShowDialog(dd2 &sub.DlgProc2 0 0 1 0 0 0 800)
opt waitmsg 1
wait 0 -WC w1
wait 0 -WC w2


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4 sub.ChangeParent id(3 hDlg) win("Dialog2")
	case IDCANCEL
ret 1


#sub DlgProc2
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4 sub.ChangeParent id(3 hDlg) win("Dialog1")
	case IDCANCEL
ret 1


#sub ChangeParent
function h hNewParent

if(SetParent(h hNewParent)) ret
int vis=IsWindowVisible(h)
if(vis) hid h
outw SetParent(h 0)
 out SetWindowLong(h GWL_HWNDPARENT HWND_MESSAGE) ;fails
 outx GetWinStyle(h 1)
 Zorder h iif(GetWinStyle(hNewParent 1)&WS_EX_TOPMOST HWND_TOPMOST HWND_NOTOPMOST) SWP_NOACTIVATE
SetWindowPos h iif(GetWinStyle(hNewParent 1)&WS_EX_TOPMOST HWND_TOPMOST HWND_NOTOPMOST) 0 0 0 0 SWP_NOSIZE|SWP_NOMOVE|SWP_NOACTIVATE
outw SetParent(h hNewParent)
outx GetWinStyle(h 1)
SetWindowPos h HWND_BOTTOM 0 0 0 0 SWP_NOSIZE|SWP_NOMOVE|SWP_NOACTIVATE|SWP_NOOWNERZORDER
SetWinStyle h WS_EX_TOPMOST 2|4|8|16
outx GetWinStyle(h 1)
if(vis) hid- h
