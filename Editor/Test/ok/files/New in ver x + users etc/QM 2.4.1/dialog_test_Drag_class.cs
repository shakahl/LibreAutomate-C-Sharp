/exe
\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dialog_test_Drag_class" &dialog_test_Drag_class)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040100 "*" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_LBUTTONDOWN goto gDrag
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

 gDrag
out
__Drag x.Init(hDlg 1)
rep
	if(!x.Next) break
	x.cursor=iif(x.mk&MK_CONTROL 2 1)
	 x.cursor=5
	 x.cursor=LoadCursor(0 +IDC_APPSTARTING)
	 outw child(x.p.x x.p.y hDlg 8)
	 OutWinMsg x.m.message x.m.wParam x.m.lParam
	 ...
if(!x.dropped) ret ;;eg user pressed Esc
out _s.getstruct(x 1)

 BEGIN PROJECT
 main_function  dialog_test_Drag_class
 exe_file  $my qm$\dialog_test_Drag_class.qmm
 flags  6
 guid  {554B709C-ACBB-41F1-9EA4-F688EA046F18}
 END PROJECT
