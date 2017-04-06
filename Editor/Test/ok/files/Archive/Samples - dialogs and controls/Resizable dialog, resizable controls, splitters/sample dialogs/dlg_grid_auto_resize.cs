\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 Auto resizes grid control columns when using DT_AutoSizeControls.

str controls = "3"
str qmg3x
if(!ShowDialog("dlg_grid_auto_resize" &dlg_grid_auto_resize &controls)) ret

 BEGIN DIALOG
 0 "" 0x90CF0AC8 0x0 0 0 217 120 "Dialog"
 3 QM_Grid 0x56031041 0x0 0 0 216 114 "0x0,0,0,0,0x0[]A,,,[]B,,,"
 END DIALOG
 DIALOG EDITOR: "" 0x2030304 "*" "" ""

ret
 messages
DlgGrid g.Init(hDlg 3)
DT_AutoSizeControls hDlg message "3s"
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_SIZE
	g.ColumnsWidthAdjust
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
