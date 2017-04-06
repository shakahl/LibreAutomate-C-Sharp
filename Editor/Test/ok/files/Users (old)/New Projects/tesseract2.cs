 Tested 2017-03-15 with the latest tesseract version 3.05-dev.
 Too much OCR errors to be useful when the image is captured from screen, with small font.
 MODI works without errors with same image.

out
 str fTes="Q:\Downloads\tesseract\tesseract.exe" ;;change this if need
str fTes="$Program Files$\Tesseract-OCR\tesseract.exe" ;;change this if need
int scale=3 ;;try to change this if recognition is poor. Tesseract is very sensitive to text size. Usually with 2-4 works best. Fastest 2, slightly slower 3, then 4, slowest 1.

 ---------------------

str fBmp="$temp$\qm_tesseract.bmp"
str fTxt="$temp$\qm_tesseract.txt"
fTes.expandpath
fBmp.expandpath
fTxt.expandpath
str language;;="lit" ;;does not work
str fBmp1.expandpath("$temp$\qm_tesseract1.bmp")
str fBmp2.expandpath("$temp$\qm_tesseract2.bmp")

 capture bitmap (optional)
if(!CaptureImageOrColor(0 0 _hwndqm fBmp)) ret
 if(!CaptureImageOnScreen(610 400 800 45 fBmp)) ret

PerfFirst
if scale!1
#if 0
	 resize with GflAx
	typelib GflAx {059321F1-207A-47A7-93A1-29CDF876FDD3} 1.0
	GflAx.GflAx g._create
	g.LoadBitmap(fBmp)
	 g.saturation(-100)
	 g.Sharpen(50)
	 g.ChangeColorDepth(1 0 1)
	if(scale!1) g.Resize(g.width*scale g.height*scale) ;;better quality and OCR than CopyImage
	g.SaveFormatName="bmp"
	g.SaveBitmap(fBmp1)
	
	PerfNext
#endif
	 resize with FreeImage. Better results.
#compile "__FreeImage"
	FI_ShowMoreErrorInfo ;;optional
	FiBitmap x
	x.Load(FIMG.FIF_BMP fBmp)
	int newWidth=FIMG.FreeImage_GetWidth(x)*scale
	int newHeight=FIMG.FreeImage_GetHeight(x)*scale
	 FiBitmap x2.Attach(FIMG.FreeImage_Rescale(x newWidth newHeight FIMG.FILTER_BSPLINE)) ;;too much filtering
	 FiBitmap x2.Attach(FIMG.FreeImage_Rescale(x newWidth newHeight 0)) ;;FIMG.FILTER_BOX, bad OCR
	FiBitmap x2.Attach(FIMG.FreeImage_Rescale(x newWidth newHeight FIMG.FILTER_LANCZOS3)) ;;good
	
	FIMG.FreeImage_AdjustContrast(x2 10)
	
	x2.Save(FIMG.FIF_BMP fBmp2)
PerfNext
 run "$program files$\IrfanView\i_view32.exe" fBmp; ret
 out F"<><image ''{fBmp}''></image>"; ret ;;does not work because of caching
 run fBmp; ret ;;nonsense

int i
for i 0 2
	if(scale!1) fBmp=iif(i fBmp2 fBmp1)
	 convert to text
	str cl.format("''%s'' ''%s'' stdout" fTes fBmp) so
	if(language.len) cl+F" -l {language}"
	 out cl
	if(RunConsole2(cl so "" 0x200)) end so
	PerfNext
	
	out so
PerfOut

 ret
str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 1210 366 "Dialog"
 3 Static 0x5400000E 0x0 0 0 1158 174 ""
 4 Static 0x5400000E 0x0 0 184 1158 180 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2040308 "*" "" "" ""

str controls = "3 4"
str sb3 sb4
sb3=fBmp1
sb4=fBmp2
if(!ShowDialog(dd &sub.DlgProc &controls)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
