 /exe 1
\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dialog_joystick" &dialog_joystick)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 Button 0x54030000 0x4 170 116 48 14 "joy"
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	if(joySetCapture(hDlg 0 0 0)) out "failed"
	
	case WM_DESTROY
	joyReleaseCapture 0
	
	case MM_JOY1BUTTONDOWN
	outx wParam
	 receives this message just for first 4 buttons
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 3
	JOYINFOEX j.dwSize=sizeof(j); j.dwFlags=JOY_RETURNBUTTONS
	if(joyGetPosEx(0 &j)) ret
	outx j.dwButtons

ret 1

 BEGIN PROJECT
 main_function  dialog_joystick
 exe_file  $my qm$\dialog_joystick.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {412B8D07-FAA2-4D81-BDE2-AE44A1B2EDA3}
 END PROJECT
