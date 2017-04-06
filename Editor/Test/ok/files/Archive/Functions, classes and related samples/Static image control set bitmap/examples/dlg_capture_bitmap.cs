\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_capture_bitmap" &dlg_capture_bitmap 0)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 515 317 "Dialog"
 3 Button 0x54032000 0x0 464 208 46 38 "Capture Bitmap"
 4 Static 0x5400000E 0x0 0 0 460 308 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030200 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 3
	__GdiHandle h
	if(!CaptureImageOrColor(&h 2 hDlg)) ret
	StaticImageControlSetBitmap id(4 hDlg) h
	
	case IDOK
	case IDCANCEL
ret 1
