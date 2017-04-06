 /DialogPrint
function hDlg hwndre $name

 Works but incorrect margins. Don't know how to calc correct.

type PRINTDLG2 lStructSize hwndOwner hDevMode hDevNames hDC Flags @nFromPage @nToPage @nMinPage @nMaxPage @nCopies [+2]hInstance lCustData lpfnPrintHook lpfnSetupHook $lpPrintTemplateName $lpSetupTemplateName hPrintTemplate hSetupTemplate
 declare PRINTDLG because it has nonstandard alignment

PRINTDLG2 pd.lStructSize=sizeof(pd)-2
pd.hwndOwner=hDlg
pd.Flags       = PD_RETURNDC|PD_USEDEVMODECOPIESANDCOLLATE|PD_NOSELECTION
pd.nCopies     = 1
pd.nFromPage   = 1
pd.nToPage     = 1
pd.nMinPage    = 1
pd.nMaxPage    = 0xFFFF
pd.hDevMode = g_psd9.hDevMode
pd.hDevNames = g_psd9.hDevNames

if(!PrintDlg(+&pd))
	if(CommDlgExtendedError()) goto ge
	ret

g_psd9.hDevMode = pd.hDevMode
g_psd9.hDevNames = pd.hDevNames

int hdc = pd.hDC

DOCINFOW di.cbSize=sizeof(di)
di.lpszDocName = @name
if(StartDocW(hdc, &di) < 0) goto ge

RECT rectMargins, rectPhysMargins;
POINT ptPage, ptDpi;

// Get printer resolution
ptDpi.x = GetDeviceCaps(hdc, LOGPIXELSX);    ;; dpi in X direction
ptDpi.y = GetDeviceCaps(hdc, LOGPIXELSY);    ;; dpi in Y direction

// Start by getting the physical page size (in device units).
ptPage.x = GetDeviceCaps(hdc, PHYSICALWIDTH);   ;; device units
ptPage.y = GetDeviceCaps(hdc, PHYSICALHEIGHT);  ;; device units

// Get the dimensions of the unprintable
// part of the page (in device units).
rectPhysMargins.left = GetDeviceCaps(hdc, PHYSICALOFFSETX);
rectPhysMargins.top = GetDeviceCaps(hdc, PHYSICALOFFSETY);

// To get the right and lower unprintable area,
// we take the entire width and height of the paper and
// subtract everything else.
rectPhysMargins.right = ptPage.x- GetDeviceCaps(hdc, HORZRES)- rectPhysMargins.left	;; total paper width// printable width// left unprintable margin
rectPhysMargins.bottom = ptPage.y- GetDeviceCaps(hdc, VERTRES)- rectPhysMargins.top

// At this point, rectPhysMargins contains the widths of the
// unprintable regions on all four sides of the page in device units.

// Take in account the page setup given by the user
RECT rectSetup;

// Convert the hundredths of millimeters (HiMetric) or
// thousandths of inches (HiEnglish) margin values
// from the Page Setup dialog to device units.
// (There are 2540 hundredths of a mm in an inch.)

str localeInfo="   "; int dn
GetLocaleInfo(LOCALE_USER_DEFAULT, LOCALE_IMEASURE, localeInfo, 3);

if(localeInfo[0] == '0') dn=2540; else dn=1000 ;; Metric system. '1' is US System
rectSetup.left = MulDiv(g_psd9.rtMargin.left, ptDpi.x, dn);
rectSetup.top = MulDiv(g_psd9.rtMargin.top, ptDpi.y, dn);
rectSetup.right	= MulDiv(g_psd9.rtMargin.right, ptDpi.x, dn);
rectSetup.bottom	= MulDiv(g_psd9.rtMargin.bottom, ptDpi.y, dn);

// Dont reduce margins below the minimum printable area
rectMargins.left	= iif(rectPhysMargins.left>rectSetup.left rectPhysMargins.left rectSetup.left);
rectMargins.top	= iif(rectPhysMargins.top>rectSetup.top rectPhysMargins.top rectSetup.top);
rectMargins.right	= iif(rectPhysMargins.right>rectSetup.right rectPhysMargins.right rectSetup.right);
rectMargins.bottom	= iif(rectPhysMargins.bottom>rectSetup.bottom rectPhysMargins.bottom rectSetup.bottom);

// rectMargins now contains the values used to shrink the printable
// area of the page.

// Convert device coordinates into logical coordinates
DPtoLP(hdc, +&rect›argins, 2);
DPtoLP(hdc, +&rectPhysMargins, 2);

// Convert page size to logical units and we're done!
DPtoLP(hdc, +&ptPage, 1);

//Create header font
TEXTMETRIC tm;
int headerLineHeight = MulDiv(10, ptDpi.y, 72);
int fontHeader = CreateFont(headerLineHeight, 0, 0, 0, FW_BOLD, 0, 0, 0, 0, 0, 0, 0, 0, "Tahoma");
SelectObject(hdc, fontHeader);
GetTextMetrics(hdc, &tm);
headerLineHeight = tm.tmHeight + tm.tmExternalLeading;

int lengthDoc = GetWindowTextLengthW(hwndre)
int lengthDocMax = lengthDoc;
int lengthPrinted = 0;

// We must substract the physical margins from the printable area
FORMATRANGE frPrint;
frPrint.hdc = hdc;
frPrint.hdcTarget = hdc;
frPrint.rc.left = rectMargins.left - rectPhysMargins.left;
frPrint.rc.top = rectMargins.top - rectPhysMargins.top;
frPrint.rc.right = ptPage.x - rectMargins.right - rectPhysMargins.left;
frPrint.rc.bottom = ptPage.y - rectMargins.bottom - rectPhysMargins.top;
frPrint.rcPage.left = 0;
frPrint.rcPage.top = 0;
frPrint.rcPage.right = ptPage.x - rectPhysMargins.left - rectPhysMargins.right - 1;
frPrint.rcPage.bottom = ptPage.y - rectPhysMargins.top - rectPhysMargins.bottom - 1;
frPrint.rc.top += headerLineHeight + (headerLineHeight / 2);

// Print each page
int pageNum = 1;
str ss;
int prevLengthPrinted=-1

rep
	 out "%i %i" lengthPrinted lengthDoc
	if(lengthPrinted>=lengthDoc or lengthPrinted<=prevLengthPrinted) break
	prevLengthPrinted=lengthPrinted
	
	StartPage(hdc);

	SetTextColor(hdc, 0x000000);
	SetBkColor(hdc, 0xffffff);
	SelectObject(hdc, fontHeader);
	int ta = SetTextAlign(hdc, TA_BOTTOM);
	RECT rcw
	rcw.left=frPrint.rc.left
	rcw.top=frPrint.rc.top - headerLineHeight - (headerLineHeight/2)
	rcw.right=frPrint.rc.right
	rcw.bottom = rcw.top + headerLineHeight;
	ExtTextOutW(hdc, frPrint.rc.left+5, frPrint.rc.top-(headerLineHeight/2), ETO_OPAQUE, &rcw, di.lpszDocName, len(di.lpszDocName), 0);
	ss=pageNum
	ExtTextOutW(hdc, frPrint.rc.right-30-(log10(pageNum)*20), frPrint.rc.top-(headerLineHeight/2), ETO_OPAQUE, 0, @ss, ss.len, 0);
	SetTextAlign(hdc, ta);
	int pen = CreatePen(0, 1, 0x000000);
	int penOld = SelectObject(hdc, pen);
	MoveToEx(hdc, frPrint.rc.left, frPrint.rc.top-(headerLineHeight/4), 0);
	LineTo(hdc, frPrint.rc.right, frPrint.rc.top-(headerLineHeight/4));
	SelectObject(hdc, penOld);
	DeleteObject(pen);

	frPrint.chrg.cpMin = lengthPrinted;
	frPrint.chrg.cpMax = lengthDoc;

	lengthPrinted = SendMessage(hwndre EM_FORMATRANGE, 1, &frPrint);

	EndPage(hdc);
	pageNum+1

SendMessage(hwndre EM_FORMATRANGE, 0, 0);

EndDoc(hdc);
DeleteDC(hdc);
DeleteObject(fontHeader);
ret

 ge
if(pd.hDC) DeleteDC(pd.hDC);
mes("Failed." "" "!");
