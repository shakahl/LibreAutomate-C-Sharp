\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str sb3
if(!ShowDialog("" &dialog_GdipLoadPictureFileData &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 Static 0x5400000E 0x0 0 0 16 16 ""
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030503 "*" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	 str s="Q:\test\il_de.bmp"
	str s="Q:\test\app_55.png"
	 str s="$documents$\foto\__kate.jpg"
	 str s="$qm$\web\images\btn_buynow_SM.gif"
	 str s="Q:\test\folder.ico"
	 __GdiHandle-- hb=GdipLoadPictureFile(s)
	__GdiHandle-- hb=GdipLoadPictureFileData(_s.getfile(s))
	SendMessage id(3 hDlg) STM_SETIMAGE IMAGE_BITMAP hb
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
