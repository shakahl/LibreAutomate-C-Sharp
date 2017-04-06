/exe
\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_magnifier" &dlg_magnifier)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2020103 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	SetTimer hDlg 1 50 0
	
	case WM_TIMER
	if(wParam=1)
		POINT p; xm p ;;get mouse pos
		int dcs(GetDC(0)) dcd(GetDC(hDlg)) ;;get screen dc and dialog dc
		StretchBlt dcd 0 0 128 128 dcs p.x p.y 16 16 SRCCOPY ;;copy from screen to dialog, and magnify 8 times, from 16 to 128
		ReleaseDC 0 dcd; ReleaseDC 0 dcs
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

 BEGIN PROJECT
 main_function  dialog_magnifier
 exe_file  $my qm$\dialog_magnifier.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {29B30590-A974-4201-8E84-0219C4D708FB}
 END PROJECT
