 /exe
 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

int-- cursor
__Hicon-- hi

#if EXE
 #exe addfile "$qm$\cur00002.cur" 211 RT_GROUP_CURSOR
 #exe addfile "$qm$\cur00002.cur" 211 RT_CURSOR
#exe addfile "" 211 RT_GROUP_CURSOR
 #exe addfile "" 31 RT_CURSOR

 #exe addfile "$qm$\mouse.ico" 133 RT_GROUP_ICON
 #exe addfile "$qm$\mouse.ico" 133 RT_ICON
 #exe addfile "" 133 RT_GROUP_ICON
 #exe addfile "" 133 RT_ICON

#exe addfile "$system$\notepad.exe" 133 RT_ICON

#endif
 cursor=LoadCursor(_hinst +211)
 cursor=GetFileIcon(":5 $windows$\Cursors\aero_busy_l.ani" 0 4)
 out cursor

hi=GetFileIcon(":133")
out hi

 _s.expandpath(":5 $system$\notepad.exe")

str controls = "3 4 5"
str si3 sb4 si5
 si3=":2 notepad.exe,0"
sb4=":1 $qm$\il_dlg.bmp"
if(!ShowDialog("" &dialog_test_drag_cursor &controls win("" "QM_Editor"))) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 Static 0x54000003 0x0 12 8 16 16 ""
 4 Static 0x5400000E 0x0 56 8 16 16 ""
 5 Static 0x54000003 0x0 10 48 16 16 ""
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	SendMessage id(5 hDlg) STM_SETICON hi 0
	
	case WM_LBUTTONDOWN
	 outw GetAncestor(hDlg GA_PARENT)
	 outw GetParent(hDlg)
	 __MinimizeDialog m.Minimize(hDlg)
	 Drag hDlg &Callback_Drag2 cursor
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

 BEGIN PROJECT
 main_function  dialog_test_drag_cursor
 exe_file  $my qm$\dialog_test_drag_cursor.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  $desktop$\test\qm.res
 on_before  
 on_after  
 on_run  
 version  
 version_csv  
 flags  23
 end_hotkey  0
 guid  {DE3B2BFF-68F1-49A2-926B-AF1EC28865B1}
 END PROJECT
