\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_button_images" &dlg_button_images)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 223 135 "Dialog"
 3 Button 0x54032000 0x0 6 4 212 16 "Bitmap with text"
 4 Button 0x54032000 0x0 6 24 212 16 "Icon with text"
 5 Button 0x54032080 0x0 6 50 212 16 "Bitmap without text"
 6 Button 0x54032040 0x0 6 70 212 16 "Icon without text"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2020105 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	 SetWindowTheme id(3 hDlg) L"" L""
	 SetWindowTheme id(4 hDlg) L"" L""
	 note that BM_SETIMAGE does not work on OS before Vista if BS_BITMAP or BS_ICON style is not set
	SendMessage(id(3 hDlg) BM_SETIMAGE IMAGE_BITMAP LoadPictureFile(":200 $my qm$\macro527.bmp" 0)) ;;can be bmp, gif of jpg
	SendMessage(id(4 hDlg) BM_SETIMAGE IMAGE_ICON GetIcon(":300 $qm$\mouse.ico"))
	SendMessage(id(5 hDlg) BM_SETIMAGE IMAGE_BITMAP LoadPictureFile(":200 $my qm$\macro527.bmp" 0)) ;;can be bmp, gif of jpg
	SendMessage(id(6 hDlg) BM_SETIMAGE IMAGE_ICON GetIcon(":300 $qm$\mouse.ico"))
	case WM_DESTROY
	DeleteObject(SendMessage(id(3 hDlg) BM_GETIMAGE IMAGE_BITMAP 0))
	DestroyIcon(SendMessage(id(4 hDlg) BM_GETIMAGE IMAGE_ICON 0))
	DeleteObject(SendMessage(id(5 hDlg) BM_GETIMAGE IMAGE_BITMAP 0))
	DestroyIcon(SendMessage(id(6 hDlg) BM_GETIMAGE IMAGE_ICON 0))
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

