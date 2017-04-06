 /exe

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 Edit 0x54030080 0x200 8 8 96 12 ""
 4 Button 0x54032000 0x0 8 24 48 14 "Button"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040400 "*" "" "" ""

str controls = "3"
str e3
e3="hh"
 if(!ShowDialog(dd &sub.DlgProc &controls)) ret
int w=ShowDialog(dd &sub.DlgProc &controls 0 1)
0
15
DestroyWindow w


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_APP
	wait 11
ret
 messages2
sel wParam
	case 4
	20
ret 1

 BEGIN PROJECT
 main_function  Dialog208
 exe_file  $my qm$\Dialog208.qmm
 flags  6
 guid  {5565CFC7-FC2D-4C7B-9C2D-069CF1F8679D}
 END PROJECT
