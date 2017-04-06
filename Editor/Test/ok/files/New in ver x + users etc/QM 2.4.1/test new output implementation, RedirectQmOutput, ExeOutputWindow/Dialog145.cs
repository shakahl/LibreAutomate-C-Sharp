 /exe
 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str e3
if(!ShowDialog("Dialog145" &Dialog145 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 Edit 0x54030080 0x200 28 20 96 14 ""
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040100 "*" "" "" ""

ret
 messages
 OutWinMsg message wParam lParam
outx message
sel message
	case WM_INITDIALOG
	ExeOutputWindow 0 "" hDlg
	case WM_DESTROY
	ExeOutputWindow 1
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

 BEGIN PROJECT
 main_function  Dialog145
 exe_file  $my qm$\Dialog145.qmm
 flags  6
 guid  {CDF22DF1-6E0A-4EEC-834A-D26835488AFD}
 END PROJECT
