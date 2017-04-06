\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 515 317 "Dialog"
 4 Static 0x5400000E 0x0 0 0 460 308 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030200 "" "" ""

str md=
 BEGIN MENU
 >Bitmap
 	Clipboard :501 0x0 0x0 Cq
 	Capture :502 0x0 0x0 Cw
 	<
 END MENU

if(!ShowDialog(dd &sub.DlgProc 0 0 0 0 0 0 0 0 0 md)) ret


#sub DlgProc
function# hDlg message wParam lParam
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 501
	OpenClipboard(hDlg)
	int h1=GetClipboardData(CF_BITMAP)
	if(h1)
		sub.Resize hDlg 4 h1
		StaticImageControlSetBitmap id(4 hDlg) h1
	CloseClipboard
	 note: don't need to delete bitmap when from clipboard
	
	case 502
	__GdiHandle h2
	if(!CaptureImageOrColor(&h2 2 hDlg)) ret
	sub.Resize hDlg 4 h2
	StaticImageControlSetBitmap id(4 hDlg) h2
	
	case IDOK
	case IDCANCEL
ret 1


#sub Resize
function hDlg cid hBitmap
BITMAP b
if(!GetObjectW(hBitmap sizeof(b) &b)) ret

siz b.bmWidth b.bmHeight id(cid hDlg)
WINDOWINFO k.cbSize=sizeof(k); GetWindowInfo hDlg &k
int dx=(k.rcWindow.right-k.rcWindow.left)-(k.rcClient.right-k.rcClient.left)
int dy=(k.rcWindow.bottom-k.rcWindow.top)-(k.rcClient.bottom-k.rcClient.top)
siz b.bmWidth+dx b.bmHeight+dy hDlg
CenterWindow hDlg
