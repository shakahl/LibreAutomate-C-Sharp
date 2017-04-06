\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str ax3SHD
ax3SHD="$documents$\Title.ppt"
if(!ShowDialog("dlg_powerpoint" &dlg_powerpoint &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 ActiveX 0x54030000 0x0 0 0 224 134 "SHDocVw.WebBrowser"
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "" "" ""

ret
 messages
typelib PowerPoint {91493440-5A91-11CF-8700-00AA0060263B} 2.8
SHDocVw.WebBrowser w
sel message
	case WM_INITDIALOG
	w._getcontrol(id(3 hDlg))
	PowerPoint.Presentation pa=w.Document
	out pa.Name
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
