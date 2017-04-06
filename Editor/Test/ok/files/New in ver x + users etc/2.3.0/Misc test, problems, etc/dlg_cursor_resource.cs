 /exe
\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 mes "" "" "q"

if(!ShowDialog("dlg_cursor_resource" &dlg_cursor_resource)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030003 "*" "" ""

ret
 messages
int- cursor=GetFileIcon(":6 $qm$\drag1.cur" 0 4)
 int- cursor=GetFileIcon(":6 $windows$\Cursors\3dsmove.cur" 0 4)
 int- cursor=GetFileIcon("$windows$\Cursors\aero_busy_l.ani" 0 4)
 int- cursor=GetFileIcon(":6 $windows$\Cursors\aero_busy_l.ani" 0 4)
 int- cursor=GetFileIcon(":6" 0 4) ;;does not work because QM needs to know file extension
  alternative way:
 str s.getfile(":6 $windows$\Cursors\aero_busy_l.ani")
 int- cursor=CreateIconFromResource(s s.len 0 0x00030000)
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	DestroyCursor cursor
	case WM_COMMAND goto messages2
	case WM_SETCURSOR
	SetCursor cursor
	ret DT_Ret(hDlg 1)
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

 BEGIN PROJECT
 main_function  dlg_cursor_resource
 exe_file  $my qm$\dlg_cursor_resource.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  22
 end_hotkey  0
 guid  {E0D39E92-9ED6-4C9E-A45D-7D55E784432C}
 END PROJECT
