\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str lb3
lb3="aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa[]b[]c[]d[]e[]f[]g[]h"
if(!ShowDialog("" &dlg_lb_column_width &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ListBox 0x54130301 0x200 0 0 96 48 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030109 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	SendMessage id(3 hDlg) LB_SETCOLUMNWIDTH 400 0
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
