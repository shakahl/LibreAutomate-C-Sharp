\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("button_bitmap_icon" &button_bitmap_icon)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54030080 0x0 8 4 48 14 "no text"
 4 Button 0x54030040 0x0 62 4 48 14 "no text"
 END DIALOG
 DIALOG EDITOR: "" 0x2010804 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	DT_Init(hDlg lParam)
	int bm=qm.LoadPictureFile("$qm$\de_ctrl.bmp" 0) ;;loads bmp, gif or jpg. With QM < 2.1.8.4, use int bm=LoadImage(0 _s.expandpath("$qm$\de_ctrl.bmp") IMAGE_BITMAP 0 0 LR_LOADFROMFILE)
	SendMessage(id(3 hDlg) BM_SETIMAGE IMAGE_BITMAP bm)
	int ic=GetIcon("mouse.ico")
	SendMessage(id(4 hDlg) BM_SETIMAGE IMAGE_ICON ic)
	ret 1
	case WM_DESTROY
	DT_DeleteData(hDlg)
	DeleteObject(SendMessage(id(3 hDlg) BM_GETIMAGE IMAGE_BITMAP 0))
	DestroyIcon(SendMessage(id(4 hDlg) BM_GETIMAGE IMAGE_ICON 0))
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK DT_Ok hDlg
	case IDCANCEL DT_Cancel hDlg
ret 1
