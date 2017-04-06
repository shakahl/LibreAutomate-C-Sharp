 FreeImage API declarations, and class FiBitmap.

 FreeImage is a graphics library, <link>http://freeimage.sourceforge.net/download.html</link>
 Download FreeImage DLL and Documentation. Don't need source. Put FreeImage.dll in QM folder.
 FreeImage is good for converting image file formats and basic image manipulation with good quality - resize, etc. Unlike many other graphics libraries like ImageMagick, you need just single dll file (~3MB), don't need to install. Easy to use, well documented.
 Other alternatives that can be easily used in QM: GDI+ (a Windows component), GflAx (a COM component, seems abandoned). Search forum.

 Class FiBitmap stores and manages FIBITMAP*. Auto-deletes when destroying the variable.
 The class wraps just basic FreeImage API functions - load, save, etc.
 All FreeImage API functions etc can be accessed through FIMG, like FIMG.FreeImage_GetWidth(x) or FIMG.FIF_BMP.
 A FiBitmap variable can be used with FreeImage API functions as FIBITMAP*.

 EXAMPLES

#compile "__FreeImage" ;;declares FIMG and FiBitmap. You can put this line in your init2 function, or in macros where used.
FI_ShowMoreErrorInfo ;;optional
FiBitmap x

str test=
 1 convert file format bmp to png
 2 capture image from screen and save as png file
 3 load png file and store the bitmap in clipboard
 4 load png file and draw in a window
 5 resize to 50%
 6 load/save from/to memory
 7 load from resources (QM 2.4.1+)
sel ShowMenu(test)
	case 1 ;;convert file format bmp to png
	x.Load(FIMG.FIF_BMP "$my qm$\test.bmp")
	x.Save(FIMG.FIF_PNG "$my qm$\test-conv.png")
	
	case 2 ;;capture image from screen and save as png file
	__GdiHandle hb
	if(!CaptureImageOrColor(hb 0)) ret
	x.FromHBITMAP(hb)
	x.Save(FIMG.FIF_PNG "$my qm$\test-capt.png")
	
	case 3 ;;load png file and store the bitmap in clipboard
	x.Load(FIMG.FIF_PNG "$my qm$\test-conv.png")
	 __GdiHandle hb2=x.ToHBITMAP ;;no, don't need to delete bitmap stored in clipboard
	int hb2=x.ToHBITMAP
	OpenClipboard 0; EmptyClipboard
	SetClipboardData CF_BITMAP hb2
	CloseClipboard
	
	case 4 ;;load png file and draw in a window
	x.Load(FIMG.FIF_PNG "$my qm$\test-conv.png")
	int hwnd=0 ;;screen. Normally you would draw in a window/dialog procedure on WM_PAINT, and use BeginPoint/EndPaint instead of GetDC/ReleaseDC.
	RECT r; SetRect &r 0 0 FIMG.FreeImage_GetWidth(x) FIMG.FreeImage_GetHeight(x)
	int hdc=GetDC(hwnd)
	x.Draw(hdc r)
	ReleaseDC hwnd hdc
	
	case 5 ;;resize to 50%
	x.Load(FIMG.FIF_BMP "$my qm$\test.bmp")
	int newWidth=FIMG.FreeImage_GetWidth(x)/2
	int newHeight=FIMG.FreeImage_GetHeight(x)/2
	FiBitmap x2.Attach(FIMG.FreeImage_Rescale(x newWidth newHeight FIMG.FILTER_BSPLINE))
	x2.Save(FIMG.FIF_BMP "$my qm$\test-siz.bmp")
	
	case 6 ;;load/save from/to memory
	str s1 s2
	s1.getfile("$my qm$\test.bmp")
	x.LoadMem(FIMG.FIF_BMP s1)
	x.SaveMem(FIMG.FIF_JPEG s2)
	s2.setfile("$my qm$\test-mem.jpg")
	
#if QMVER>=0x02040100
	case 7 ;;load from resources
	s1.getfile("$my qm$\test.bmp")
	 _qmfile.ResourceAdd(+-1 "test FiBitmap.bmp" s1 s1.len) ;;add a bitmap resource for testing
	 x.Load(FIMG.FIF_BMP "resource:test FiBitmap.bmp")
	s1.encrypt(32); _qmfile.ResourceAdd(+-1 "image:test FiBitmap" s1 s1.len) ;;add a compressed bitmap resource for testing
	x.Load(FIMG.FIF_BMP "image:test FiBitmap")
	x.Save(FIMG.FIF_PNG "$my qm$\test-res.png")
#endif
	
