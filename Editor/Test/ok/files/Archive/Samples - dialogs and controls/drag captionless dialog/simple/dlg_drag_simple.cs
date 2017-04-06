\Dialog_Editor

 The simplest way to drag a captionless dialog - return HTCAPTION on WM_NCHITTEST.
 However it is not perfect, because whole dialog will have incorrect hit test value, which is used by some programs, including QM mouse triggers.

function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_drag_simple" &dlg_drag_simple)) ret

 BEGIN DIALOG
 0 "" 0x90400AC8 0x0 0 0 220 133 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NCHITTEST ret DT_Ret(hDlg HTCAPTION)
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
