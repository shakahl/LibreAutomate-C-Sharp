 /exe
\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str si3
 si3="files.ico"
si3=":5 files.ico"
if(!ShowDialog("Dialog119" &Dialog119 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 Static 0x54000003 0x0 0 119 16 16 ""
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030404 "*" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	out GetExeResHandle
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

 BEGIN PROJECT
 main_function  Dialog119
 exe_file  $my qm$\Dialog119.qmm
 flags  22
 guid  {05A981FA-EDC5-4B4F-A862-B69B13633D2D}
 END PROJECT
