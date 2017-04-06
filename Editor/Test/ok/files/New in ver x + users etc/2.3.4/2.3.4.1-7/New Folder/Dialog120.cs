/exe
 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str sb3
 sb3=":5 $qm$\il_qm.bmp"
 sb3=":4 $documents$\foto\__kate.jpg"
sb3=":5 q:\test\app_55.png"
if(!ShowDialog("Dialog120" &Dialog120 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 Static 0x5400000E 0x0 34 38 16 16 ""
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030405 "*" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	DT_SetBackgroundImage hDlg ":4 $documents$\foto\__kate.jpg"
	 DT_SetBackgroundImage hDlg ":5 q:\test\app_55.png"
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

 BEGIN PROJECT
 main_function  Dialog120
 exe_file  $my qm$\Dialog120.qmm
 flags  22
 guid  {190781D7-03A6-4B3D-92AE-B2A72C331A50}
 END PROJECT
