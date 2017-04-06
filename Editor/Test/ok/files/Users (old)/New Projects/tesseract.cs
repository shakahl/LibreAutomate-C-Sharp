 Tested 2017-03-15 with the latest tesseract version 3.05-dev.
 Too much OCR errors to be useful when the image is captured from screen, with small font.
 MODI works without errors with same image.

out
 change these values if need
str fTes.expandpath("$Program Files$\Tesseract-OCR\tesseract.exe")
int scale=3 ;;how much to resize. Tesseract is very sensitive to text size. Usually with 2-4 works best. Fastest 2, slightly slower 3, then 4, slowest 1.
int captureNow=1 ;;if 0, will use previously captured file

 ---------------------

str language;;="lit" ;;does not work, maybe because console text cannot be unicode, need to write it to file instead
str fBmp.expandpath("$temp$\qm_tesseract.bmp") ;;this macro captures screen image and saves it in this temporary file
str fBmp2.expandpath("$temp$\qm_tesseract2.bmp") ;;this macro resizes the image, saves in this file, and passes this file to tesseract.exe

 capture screen image and save in file fBmp
if captureNow
	if(!CaptureImageOrColor(0 0 _hwndqm fBmp)) ret
	 if(!CaptureImageOnScreen(610 400 800 45 fBmp)) ret

PerfFirst ;;measure speed
 resize with FreeImage. Also tried GflAx, worse.
#compile "__FreeImage"
if scale>1
	FI_ShowMoreErrorInfo ;;optional
	FiBitmap x
	x.Load(FIMG.FIF_BMP fBmp)
	int newWidth=FIMG.FreeImage_GetWidth(x)*scale
	int newHeight=FIMG.FreeImage_GetHeight(x)*scale
	 FiBitmap x2.Attach(FIMG.FreeImage_Rescale(x newWidth newHeight FIMG.FILTER_BSPLINE)) ;;too much filtering
	 FiBitmap x2.Attach(FIMG.FreeImage_Rescale(x newWidth newHeight 0)) ;;FIMG.FILTER_BOX, bad OCR
	FiBitmap x2.Attach(FIMG.FreeImage_Rescale(x newWidth newHeight FIMG.FILTER_LANCZOS3)) ;;good
	
	FIMG.FreeImage_AdjustContrast(x2 10)
	
	fBmp=fBmp2
	x2.Save(FIMG.FIF_BMP fBmp)
	PerfNext ;;measure speed

 OCR (convert image to text)
str cl.format("''%s'' ''%s'' stdout" fTes fBmp) so
if(language.len) cl+F" -l {language}"
 out cl
if(RunConsole2(cl so "" 0x200)) end so
PerfNext ;;measure speed
PerfOut ;;measure speed
out so ;;show results

ret ;;disable this line if want to see the image passed to tesseract.exe

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 1000 500 "Shows the resized image for 3 seconds"
 3 Static 0x5400000E 0x0 0 0 1158 174 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2040308 "*" "" "" ""

str controls = "3"
str sb3
sb3=fBmp
if(!ShowDialog(dd &sub.DlgProc &controls)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	int x y w h
	GetWorkArea x y w h
	MoveWindow hDlg x y w h 0
	SetTimer hDlg 1 3000 0
	case WM_TIMER
	clo hDlg
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
