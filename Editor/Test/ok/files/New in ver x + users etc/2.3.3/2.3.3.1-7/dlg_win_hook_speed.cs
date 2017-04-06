 /exe
\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_win_hook_speed" &dlg_win_hook_speed)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 Button 0x54032000 0x0 6 6 48 14 "Create"
 4 Button 0x54032000 0x0 60 6 48 14 "Destroy"
 END DIALOG
 DIALOG EDITOR: "" 0x2030301 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	TO_Show hDlg "-3 4" 0
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 3
	ARRAY(int)-- a.create(1000)
	int i
	Q &q
	for i 0 a.len
		a[i]=CreateWindowEx(0 "Static" 0 WS_CHILD 0 0 100 20 hDlg 0 _hinst 0)
	Q &qq; outq
	 outw a[0]
	TO_Show hDlg "-3 4" 1
	case 4
	Q &q
	for i 0 a.len
		DestroyWindow(a[i])
	Q &qq; outq
	TO_Show hDlg "-3 4" 0
ret 1

 BEGIN PROJECT
 main_function  dlg_win_hook_speed
 exe_file  $my qm$\dlg_win_hook_speed.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {063009AC-2B1E-457F-8D0C-82A452F05343}
 END PROJECT
