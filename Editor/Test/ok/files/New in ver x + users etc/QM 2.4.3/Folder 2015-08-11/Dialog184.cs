 \Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 Edit 0x54030080 0x200 16 12 96 14 ""
 4 ComboBox 0x54230243 0x0 12 40 110 213 ""
 5 ComboBoxEx32 0x54230042 0x0 16 60 96 214 ""
 6 QM_Grid 0x56031041 0x200 124 8 96 48 "0x20,0,0,0x2,0x10[]A,,,[]B,,,[]C,,24,"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040301 "*" "" "" ""

 str controls = "3 1"
 str e3 b1OK
 type Tfff ~controls ~e3 ~b1OK
 Tfff d.controls="3 1"
 d.e3="kkk"
str controls = "3 4 5 6"
str e3 cb4 cbe5 qmg6x
cbe5="hhh"
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
