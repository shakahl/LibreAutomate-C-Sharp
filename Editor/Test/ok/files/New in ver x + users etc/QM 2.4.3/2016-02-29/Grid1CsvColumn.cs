
str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 QM_Grid 0x56031041 0x200 0 0 132 110 "0x0,0,0,0x0,0x0[]A,100%,,[]B,0,7,"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040308 "*" "" "" ""

str controls = "3"
str qmg3x
qmg3x=
 test_1.txt         ,   c:\abc\test_1.txt
 test_2.txt         ,   c:\abc\subfolder\test_2.txt
if(!ShowDialog(dd &sub.DlgProc &controls)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
