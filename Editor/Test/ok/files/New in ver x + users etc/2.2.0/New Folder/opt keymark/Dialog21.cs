\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str e3
if(!ShowDialog("Dialog21" &Dialog21 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 382 169 "Form"
 3 Edit 0x54231044 0x200 0 14 382 156 ""
 1 Button 0x54030001 0x4 282 0 48 14 "OK"
 2 Button 0x54030000 0x4 332 0 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2020000 "" ""

ret
 messages
sel message
	case WM_LBUTTONDOWN
	Q &q
	rep(95) key a
	Q &qq
	outq
	key Y

	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
