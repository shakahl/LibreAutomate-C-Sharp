 /
function# hDlg message

MSHTML.IHTMLElement el
__MinimizeDialog m
__OnScreenRect osr

sel message
	case WM_LBUTTONDOWN
	m.Minimize(hDlg)
	__Drag d.Init(hDlg 1)
	rep() sub.Rect osr; if(!sub_to.DragTool_Loop(d)) break
	if(!d.dropped) ret
	
	case WM_RBUTTONDOWN
	sel sub_to.DragTool_Menu(hDlg "{+}" 5)
		case 102 int shiftCapture=1
		
	if(!shiftCapture) ret
	opt waitmsg 1
	m.Minimize(hDlg)
	rep
		0.01
		ifk(S) break; else ifk(Z) ret
		POINT p pp; xm p; if(memcmp(&p &pp 8)) pp=p; else continue
		sub.Rect osr

sub.Rect osr el
m.Restore
if(!el) ret

type ___EH_FRAME hwnd MSHTML.IHTMLDocument2'doc str'frame
___EH_FRAME f
f.doc=el.document
f.hwnd=sub_Htm.GetDocHwnd(f.doc)
sub.Frame f

EH_Fill hDlg f.doc el f.hwnd f.frame 0


#sub Rect
function __OnScreenRect&osr [MSHTML.IHTMLElement&get]

POINT p; xm p
Htm el.FromXY(p.x p.y)

RECT r; int osrFlags
if(&get) get=el; osrFlags=2
else el.GetRect(r); err goto erase

osr.Show(osrFlags &r)
ret
err+
	 erase
osr.Show(3)


#sub Frame
function! ___EH_FRAME&f [MSHTML.IHTMLDocument2'doc]

 Gets frame path into f.frame.
 doc must be 0.

if !doc
	doc=htm(f.hwnd) ;;top-level (f.doc is top-level or frame)
	if(doc=f.doc) ret 1

err-
IOleContainer oc=+doc
IEnumUnknown eu
oc.EnumObjects(OLECONTF_EMBEDDINGS &eu)
err+ ret
int i
rep
	IUnknown u=0
	eu.Next(1 &u &_i); if(!_i) break
	err-
	SHDocVw.WebBrowser wb=u
	doc=wb.Document
	i+1
	if doc=f.doc
		f.frame=i
		ret 1
	if sub.Frame(f doc)
		f.frame-"/"; f.frame-i
		ret 1
	err+
