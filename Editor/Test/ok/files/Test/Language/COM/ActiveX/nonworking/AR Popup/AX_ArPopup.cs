\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages
typelib ARPopUpMenuCtrl {32C5D4A3-2F38-11D4-ABD7-000000000000} 1.1

if(!ShowDialog("AX_ArPopup" &AX_ArPopup)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ActiveX 0x54030000 0x0 10 10 96 48 "ARPopUpMenuCtrl.ARPopUpMenu"
 END DIALOG
 DIALOG EDITOR: "" 0x2020008 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	ARPopUpMenuCtrl.ARPopUpMenu ar3._getcontrol(id(3 hDlg))
	ar3._setevents("ar3___ARPopUpMenu")
	BSTR t("text") k("key") d("des") tag("")
	ar3.AddMenuItem(t k d tag)
	FLOAT x y
	ar3.ShowMenu(x y)
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
