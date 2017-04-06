 /
function# [$text]

 Covers screen while the macro is running.
 Returns curtain window handle.

 text - text to display.

 REMARKS
 By default, covers only the primary monitor. To cover monitor 2, before insert: _monitor=2


str dd=
 BEGIN DIALOG
 0 "" 0x81C00048 0x88 0 0 227 154 "Please wait..."
 END DIALOG

opt waitmsg 2
str controls="0"
str f=iif(!empty(text) text "Please wait...")
ret ShowDialog(dd &sub.Dlg &controls 0 1)


#sub Dlg
function# hDlg message wParam lParam
sel message
	case WM_INITDIALOG
	SetWindowTheme hDlg L"" L""
	SetWinStyle hDlg WS_EX_LAYERED|WS_EX_TRANSPARENT|WS_EX_NOACTIVATE 1|4
	SetLayeredWindowAttributes(hDlg 0xff 0 1)
	SetWindowPos hDlg HWND_TOPMOST 0 0 0 0 SWP_NOSIZE|SWP_NOMOVE|SWP_SHOWWINDOW|SWP_NOACTIVATE
