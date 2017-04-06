 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 BEGIN DIALOG
 0 "" 0x80000848 0x8000080 0 0 227 150 "TextDrop"
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "AddTextDrop" "" ""

ret
 messages
 OutWinMsg message wParam lParam
int hwndControl=DT_GetParam(hDlg)
sel message
	case WM_INITDIALOG
	RECT r; GetWindowRect hwndControl &r
	SetWindowPos hDlg 0 r.left r.top r.right-r.left r.bottom-r.top SWP_NOZORDER|SWP_NOACTIVATE
	Transparent hDlg 64
	SetWindowPos hDlg 0 0 0 0 0 SWP_SHOWWINDOW|SWP_NOSIZE|SWP_NOMOVE|SWP_NOZORDER|SWP_NOACTIVATE
	SetTimer hDlg 1 1000 0
	QmRegisterDropTarget(hDlg hDlg 0)
	
	case WM_QM_DRAGDROP
	QMDRAGDROPINFO& di=+lParam
	sel wParam
		case 3 ;;drop
		str s
		if(!di.GetText(s)) ret
		s.setwintext(hwndControl)
		ret DT_Ret(hDlg 1)
	
	case WM_TIMER
	sel wParam
		case 1
		if(!IsWindow(hwndControl)) DestroyWindow hDlg; ret
		 also should move dialog if hwndControl moved; hide if hidden or minimized, etc
ret

 Should also somehow relay mouse and some other messages to hwndControl,
 because now it is covered by this dialog and therefore is like disabled.
 Cannot make this dialog completely transparent, because dag/drop will not work.
 Easier would be to move this dialog beside the control, not to cover the control.
 Need much more code to make this work well in all cases.
