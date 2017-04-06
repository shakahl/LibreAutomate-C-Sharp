\Dialog_Editor

 Run this macro.
 It shows a window that displays screen image under it, with one changed color.
 You can move and resize the window. Move it over the area where you want to change color.

 Replace these colors to your colors. This is an example to change white to yellow.
int color1=0xffffff ;;color to be changed
int color2=0x00ffff ;;color to change to
int updatePeriod=1000 ;;milliseconds
def DRCPC_USE_TCC 1 ;;if 1, compiles the slowest code part to C. It makes faster. Set to 0 if have problems with it.

 ____________________________________________

str dd=
 BEGIN DIALOG
 0 "" 0x90CC0AC8 0xC0088 0 0 224 136 "Color Changer"
 END DIALOG
 DIALOG EDITOR: "" 0x2040200 "*" "" "" ""

color1=((color1&0xff)<<16)|((color1&0xff0000)>>16)|(color1&0xff00)
color2=((color2&0xff)<<16)|((color2&0xff0000)>>16)|(color2&0xff00)

if(!ShowDialog(dd &sub.DlgProc 0)) ret


#sub DlgProc v
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	Transparent hDlg 255 GetSysColor(COLOR_BTNFACE)
	SetTimer hDlg 1 updatePeriod 0
	
	case WM_TIMER
	sel wParam
		case 1 sub.TimerProc hDlg
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1


#sub TimerProc v
function hDlg
 KillTimer hDlg 1

 get dialog DC and rectangle. Erase.
__Hdc dc.Init(hDlg)
RECT r; GetClientRect hDlg &r
FillRect dc &r GetSysColorBrush(COLOR_BTNFACE)

 copy screen image under dialog to a memory DC. Slow, but there are no faster ways.
MapWindowPoints hDlg 0 +&r 2
int cx(r.right-r.left) cy(r.bottom-r.top)
__MemBmp m.Create(cx cy 1 r.left r.top)

 get color bits
ARRAY(int) a.create(cx cy)
int* b=&a[0 0]
BITMAPINFO bi; int _1 _2
BITMAPINFOHEADER& h=bi.bmiHeader
h.biSize=sizeof(h)
h.biBitCount=32; h.biWidth=cx; h.biHeight=-cy; h.biPlanes=1
if(GetDIBits(m.dc m.bm 0 cy b +&h DIB_RGB_COLORS)!=cy) ret

 replace color. Compile to C by default. Would be too slow in QM.
int n(cx*cy) alpha(b[0]&0xff000000) c1(color1|alpha) c2(color2|alpha)
#if DRCPC_USE_TCC
	__Tcc+ __tcc_drspc
	if(!__tcc_drspc.f) __tcc_drspc.Compile("" "ReplaceColor")
	call __tcc_drspc.f b n c1 c2
#else
	int i
	for i 0 n
		if(b[i]=c1) b[i]=c2
#endif

 draw the modified color bits on dialog
if(!SetDIBitsToDevice(dc 0 0 cx cy 0 0 0 cy b &bi DIB_RGB_COLORS)) ret


#ret
void ReplaceColor(int* b, int n, int c1, int c2)
{
int* be=b+n;
for(; b<be; b++) if(*b==c1) *b=c2;
}
