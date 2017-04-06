/exe
\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("" &Dialog_font_sample6 0 _hwndqm)) ret

 BEGIN DIALOG
 0 "" 0x10CF0A40 0x100 0 0 167 133 "Dialog Fonts"
 3 Static 0x54000000 0x0 112 36 48 13 "Text"
 4 Button 0x54012003 0x0 112 52 48 13 "Check"
 10 Button 0x54020007 0x20 112 70 44 26 "Sssssss"
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	 out
	
	DT_SetTextColor hDlg 0xff
	 DT_SetBackgroundColor hDlg 0 0xff8080
	DT_SetBackgroundColor hDlg 1 0xff8080 0xffffff
	 DT_SetBackgroundImage hDlg "$qm$\il_icons.bmp"
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 12
ret 1
 BEGIN PROJECT
 main_function  Dialog_font_sample6
 exe_file  $my qm$\Dialog_font_sample6.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {4E7E8AB5-250C-404A-BFB6-CCC1051E0555}
 END PROJECT
