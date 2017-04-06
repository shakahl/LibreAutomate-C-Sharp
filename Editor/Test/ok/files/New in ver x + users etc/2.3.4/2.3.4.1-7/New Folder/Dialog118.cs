 /exe
 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 ExeQmGridDll
 if(0) SendMail "" "" ""

str controls = "3"
str qmg3x
if(!ShowDialog("Dialog118" &Dialog118 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 QM_Grid 0x56031041 0x200 0 87 96 48 "0x0,0,0,0,0x0[]A,,,[]B,,,"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030404 "*" "" "" ""

ret
 messages
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

 BEGIN PROJECT
 main_function  Dialog118
 exe_file  $my qm$\Dialog118.qmm
 flags  6
 guid  {F0C766CE-3304-44BE-B856-E3BD7DD49742}
 END PROJECT
