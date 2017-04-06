\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_ole_drop_text" &dlg_ole_drop_text 0 _hwndqm 0 0 0 0 1 -1)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 223 135 "Dialog"
 END DIALOG
 DIALOG EDITOR: "" 0x2020006 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	QmRegisterDropTarget(hDlg hDlg 0)
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_QM_DRAGDROP
	QMDRAGDROPINFO& di=+lParam
	sel wParam
		case 3 ;;drop
		str s
		if(!di.GetText(s)) ret
		out s
		ret DT_Ret(hDlg 1)
	
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
