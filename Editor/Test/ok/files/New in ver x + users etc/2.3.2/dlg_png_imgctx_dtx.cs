\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

dll "qm.exe" #LoadPictureFile_IImgCtx $imgFile
dll "qm.exe" #LoadPictureFile_DXT $imgFile

str controls = "3"
str sb3
if(!ShowDialog("dlg_png" &dlg_png &controls _hwndqm)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 328 168 "Dialog"
 3 Static 0x5400000E 0x0 242 102 66 58 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030201 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	 str f="$program files$\Last.fm\data\app_55.png"
	str f="q:\test\app_55.png"
	 Q &q
	__GdiHandle- hb=LoadPictureFile_IImgCtx(f)
	 __GdiHandle- hb=LoadPictureFile_DXT(f)
	 Q &qq; outq
	StaticImageControlSetBitmap id(3 hDlg) hb
	
	__MemBmp- mb.Attach(LoadPictureFile_IImgCtx(f))
	 __MemBmp- mb.Attach(LoadPictureFile_DXT(f))
	
	BITMAP b; if(!GetObject(mb.bm sizeof(b) &b)) ret
	ARRAY(int) a.create(b.bmHeight*b.bmWidth)
	if(!GetBitmapBits(mb.bm a.len*4 &a[0])) ret
	outx a[0]
	outx a[b.bmWidth*(b.bmHeight/2)+(b.bmWidth/2)] ;;center
	
	case WM_PAINT
	PAINTSTRUCT ps
	BeginPaint hDlg &ps
	BitBlt ps.hDC 0 0 100 100 mb.dc 0 0 SRCCOPY
	EndPaint hDlg &ps
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
