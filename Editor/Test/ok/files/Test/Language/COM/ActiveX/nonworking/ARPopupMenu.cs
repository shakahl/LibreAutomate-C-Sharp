\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 When clicked, creates new control, and therefore does not work on QM.

typelib ARPopUpMenuCtrl {32C5D4A3-2F38-11D4-ABD7-000000000000} 1.1

if(!ShowDialog("ARPopupMenu" &ARPopupMenu 0)) ret

 BEGIN DIALOG
 0 "" 0x10C80A44 0x100 0 0 223 135 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 5 ActiveX 0x54000000 0x4 2 10 22 20 "ARPopUpMenuCtrl.ARPopUpMenu"
 3 Button 0x54032000 0x0 32 8 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2010700 "*" ""

ret
 messages
sel message
	case WM_INITDIALOG DT_Init(hDlg lParam)
	ARPopUpMenuCtrl.ARPopUpMenu ar5._getcontrol(id(5 hDlg))
	 ar5._setevents("ar5___ARPopUpMenu")
	BSTR te("Text") ke("Key") de("descr") ta("tag")
	ar5.AddMenuItem(&te &ke &de &ta)
	
	ret 1
	case WM_DESTROY DT_DeleteData(hDlg)
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 3
	ar5._getcontrol(id(5 hDlg))
	FLOAT x(30) y(30)
	ar5.ShowMenu(&x &y)
	case IDOK DT_Ok hDlg
	case IDCANCEL DT_Cancel hDlg
ret 1
