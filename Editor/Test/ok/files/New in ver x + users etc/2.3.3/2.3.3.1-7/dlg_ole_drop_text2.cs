\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 4"
str ax3SHD rew4
 ax3SHD = "http://emedicine.medscape.com/article/176595-overview"
ax3SHD = "http://www.quickmacros.com"

if(!ShowDialog("dlg_ole_drop_text2" &dlg_ole_drop_text2 &controls _hwndqm)) ret

 BEGIN DIALOG
 1 "" 0x90C80A44 0x100 0 0 284 173 "TextCapture"
 3 ActiveX 0x54030000 0x0 0 0 284 124 "SHDocVw.WebBrowser"
 4 RichEdit20W 0x54233044 0x200 0 124 96 48 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG 
	
	QmRegisterDropTarget(hDlg hDlg 1)
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_QM_DRAGDROP
	QMDRAGDROPINFO& di=+lParam
	sel wParam
		case 3 ;;drop
		
		str s
		if(!di.GetText(s)) ret 
		
		out s 
		
		 outx di.effect
		ret DT_Ret(hDlg 1)
		
		case 0
		ifk(C) di.effect=1; else if(di.effect&2) di.effect=2
		ret DT_Ret(hDlg 1)
	
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1