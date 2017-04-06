 /exe
 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("" &dialog_test_drag_cursor2 0 win("" "QM_Editor"))) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	
	case WM_LBUTTONDOWN
	 __MinimizeDialog m.Minimize(hDlg)
	Drag hDlg &Callback_Drag2 4
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

 BEGIN PROJECT
 main_function  dialog_test_drag_cursor2
 exe_file  $my qm$\dialog_test_drag_cursor2.qmm
 icon  <default>
 manifest  $qm$\default.exe.manifest
 version_csv  FileVersion,5.6 beta[]LegalCopyright,cccc
 flags  23
 guid  {190E6506-3137-42A4-ABE7-6EF6453D0180}
 END PROJECT
