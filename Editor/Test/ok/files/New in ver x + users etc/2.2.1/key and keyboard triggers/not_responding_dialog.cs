 /exe
\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("not_responding_dialog" &not_responding_dialog 0 0 0 0 0 0 -1 1)) ret

 BEGIN DIALOG
 0 "" 0x10C80A44 0x100 0 0 223 135 "Dialog"
 5 Edit 0x54231044 0x200 0 28 224 86 ""
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 10 10 48 14 "Start"
 4 Static 0x54000000 0x0 64 8 122 13 "Press Ctrl to stop"
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
sel wParam
	case 3
	_s.setwintext(id(5 hDlg))
	act id(5 hDlg)
	0
	 g1
	rep
		ifk(F11) break
	case IDOK
	case IDCANCEL
	
	 case EN_CHANGE<<16|5
	 _s.getwintext(id(5 hDlg))
	 if(_s.len) goto g1
ret 1

 BEGIN PROJECT
 main_function  not_responding_dialog
 exe_file  $my qm$\not_responding_dialog.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {C46FC7F6-FCD2-4D5F-97B6-9BC84664D924}
 END PROJECT
