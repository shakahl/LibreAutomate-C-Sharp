/exe
 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 5"
str e3 c5Slo
hDlg=ShowDialog("dlg_test_tsm" &dlg_test_tsm &controls 0 1 0 0 0 -1)
rep
	MSG m
	if(!GetMessage(&m 0 0 0)) break
	sel m.message
		case WM_KEYUP
		out F"received {m.wParam}"
		case WM_CHAR
		case WM_TIMER
		 0.1
		if(but(5 hDlg)) wait RandomNumber*0.1
	
	TranslateMessage &m
	DispatchMessage &m


 BEGIN DIALOG
 0 "" 0x90C80AC8 0x8 0 0 223 135 "Test TSM"
 3 Edit 0x54231044 0x200 0 0 224 114 ""
 2 Button 0x54030000 0x4 168 118 48 14 "Cancel"
 4 Button 0x54032000 0x0 6 118 48 14 "Clear"
 5 Button 0x54012003 0x0 86 120 48 12 "Slow"
 END DIALOG
 DIALOG EDITOR: "" 0x2030301 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	SetTimer hDlg 1 100 0
	case WM_DESTROY
	PostQuitMessage 0
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4 _s.setwintext(id(3 hDlg)); SetFocus id(3 hDlg)
	case 5 but 4 hDlg
ret 1

 BEGIN PROJECT
 main_function  dlg_test_tsm
 exe_file  $my qm$\dlg_test_tsm.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  MakeExeCloseRunning
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {68F5DA14-087D-476F-982A-17DE753DD505}
 END PROJECT
