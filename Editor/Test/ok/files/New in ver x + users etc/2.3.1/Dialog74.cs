 /exe
\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 4"
str si3 si4
if(!ShowDialog("Dialog74" &Dialog74 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "ąčę"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Static 0x54000003 0x0 0 0 16 16 ""
 4 Static 0x54000003 0x0 104 0 16 16 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030102 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	def IDI_SHIELD          32518
	__Hicon- h1=LoadIconW(0 +IDI_INFORMATION)
	 __Hicon- h2=LoadIconW(0 +IDI_SHIELD)
	__Hicon- h2=LoadIconW(0 +106)
	
	SendMessage id(3 hDlg) STM_SETICON h1 0
	SendMessage id(4 hDlg) STM_SETICON h2 0
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

 BEGIN PROJECT
 main_function  Dialog74
 exe_file  $my qm$\Dialog74.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {202470DC-6C95-421B-B4A8-329A2A2BCE5B}
 END PROJECT
