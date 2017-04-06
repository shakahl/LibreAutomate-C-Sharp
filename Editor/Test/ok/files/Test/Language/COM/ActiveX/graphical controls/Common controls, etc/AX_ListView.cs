\Dialog_Editor
typelib MSComctlLib {831FDD16-0C5C-11D2-A9FC-0000F8754DA1} 2.0
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("AX_ListView" &AX_ListView)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ActiveX 0x54030000 0x0 194 2 26 26 "MSComctlLib.ImageList {2C247F23-8591-11D1-B16A-00C0F0283628}"
 4 ActiveX 0x54030000 0x0 0 0 116 86 "MSComctlLib.ListView {BDD1F04B-858B-11D1-B16A-00C0F0283628}"
 END DIALOG
 DIALOG EDITOR: "" 0x2020009 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	MSComctlLib.ImageList im3
	im3._getcontrol(id(3 hDlg))
	im3.ImageHeight=16; im3.ImageWidth=16
	stdole.StdPicture p
	stdole.LoadPicture("q:\app\mouse.ico" 16 16 IMAGE_ICON &p)
	VARIANT vic=p
	im3.ListImages.Add(@ @ vic)
	vic=0
	
	MSComctlLib.ListView li4
	li4._getcontrol(id(4 hDlg))
	li4.SmallIcons=im3
	VARIANT vt="Text"
	li4.ListItems.Add(@ @ vt @ vic)
	
	 no error but does not show icons
	
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
