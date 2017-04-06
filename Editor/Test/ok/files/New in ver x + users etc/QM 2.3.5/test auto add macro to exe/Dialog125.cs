 /exe
\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "4"
str e4
if(!ShowDialog("" &Dialog125 &controls 0 0 0 0 0 0 0 "" "Dialog125")) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 4 Edit 0x54030080 0x200 51 47 96 15 ""
 3 Static 0x54000000 0x0 52 32 48 13 "Text"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030500 "*" "" "" ""

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
	case else outx wParam
ret 1

 BEGIN MENU
 >&File
	 &Test : 3 0 0 Cm
	 <
 END MENU

 BEGIN PROJECT
 main_function  Dialog125
 exe_file  $my qm$\Dialog125.qmm
 flags  6
 guid  {AF6C930C-A295-466C-B917-81C086FAB401}
 END PROJECT
