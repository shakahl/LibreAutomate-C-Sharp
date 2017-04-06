 /exe
\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str e3
e3=iif(_winnt=6 300 100)
if(!ShowDialog("memory_stress" &memory_stress &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 91 41 "Mem Stress"
 3 Edit 0x54030080 0x200 4 4 42 14 ""
 4 Static 0x54000000 0x0 50 6 32 12 "MB"
 5 Button 0x54032000 0x0 4 24 42 14 "Start"
 6 Button 0x54032000 0x0 50 24 38 14 "Stop"
 END DIALOG
 DIALOG EDITOR: "" 0x2020100 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
int he=id(3 hDlg)
sel wParam
	case 5
	EnableWindow he 0
	mac "eat_memory2" "" _s.getwintext(he)
	
	case 6
	 g1
	shutdown -6 0 "eat_memory2"
	EnableWindow he 1
	
	case IDOK
	case IDCANCEL goto g1
ret 1

 BEGIN PROJECT
 main_function  memory_stress
 exe_file  $my qm$\memory_stress.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {EF34B5A2-2AD3-4EEF-A0A8-D573F9424C3E}
 END PROJECT
