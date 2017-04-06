 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

typelib MOZILLACONTROLLib {1339B53E-3453-11D2-93B9-000000000000} 1.0

 BEGIN DIALOG
 0 "" 0x10C80A44 0x100 0 0 317 135 "Form"
 3 ActiveX 0x54000000 0x4 2 2 312 114 "MOZILLACONTROLLib.MozillaBrowser"
 4 Button 0x54032001 0x4 2 118 48 14 "QM"
 5 Button 0x54032000 0x4 52 118 48 14 "Google"
 1 Button 0x54030001 0x0 266 118 48 14 "OK"
 END DIALOG
 DIALOG EDITOR: "" 0x2010703 "" ""

ret
 messages
sel message
	case WM_INITDIALOG DT_Init(hDlg lParam)
	MOZILLACONTROLLib.MozillaBrowser a
	a._getcontrol(id(3 hDlg))
	a.Navigate("http://www.google.com")

	ret 1
	case WM_DESTROY DT_DeleteData(hDlg)
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4
	a._getcontrol(id(3 hDlg))
	a.Navigate("http://www.quickmacros.com")
	case 5
	a._getcontrol(id(3 hDlg))
	a.Navigate("http://www.google.com")
	
	case IDOK DT_Ok hDlg
	case IDCANCEL DT_Cancel hDlg
ret 1
