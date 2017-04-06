\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 Does not accept the Picture argument.

typelib MBListEx {5CC4270F-C421-11D3-8ED1-A112977A2C74} 1.0 0 1

if(!ShowDialog("DlgListEx" &DlgListEx 0)) ret

 BEGIN DIALOG
 0 "" 0x10C80A44 0x100 0 0 223 135 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ActiveX 0x54000000 0x0 4 2 74 78 "MBListEx.ListEx"
 END DIALOG
 DIALOG EDITOR: "" 0x2010700 "*" ""

ret
 messages
sel message
	case WM_INITDIALOG DT_Init(hDlg lParam)
	MBListEx.ListEx li3._getcontrol(id(3 hDlg))
	 MBListEx.General g.
	BSTR txt="item"
	 stdole.StdFont font=li3.Font
	stdole.Font font=li3.Font
	stdole.StdPicture pict._create
	out pict
	li3.AddItem(txt -1 -1 -1 font +&pict -1 "" -1)
	
	ret 1
	case WM_DESTROY DT_DeleteData(hDlg)
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK DT_Ok hDlg
	case IDCANCEL DT_Cancel hDlg
ret 1
