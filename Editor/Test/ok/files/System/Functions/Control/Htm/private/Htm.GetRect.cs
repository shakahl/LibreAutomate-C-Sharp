function RECT&r

 Gets rectangle in screen.
 Same as Htm.Location, but stores in RECT variable.
 This function is hidden, but can be used as public.
 Added in: QM 2.3.6.


opt noerrorshere 1
MSHTML.IHTMLElement2 el2
MSHTML.IHTMLDocument2 doc=el.document

sel el.tagName 1
	case "AREA" ;;DOM functions get incorrect offsets
	Acc area=acc(el)
	area.Navigate("parent") ;;container IMG

 get location
if area.a ;;in container IMG
	r.left=el.offsetLeft; r.top=el.offsetTop
	if(_iever<0x900) el2=htm(area); r.left+el2.clientLeft; r.top+el2.clientTop
	r.right=r.left+el.offsetWidth; r.bottom=r.top+el.offsetHeight
else ;;in doc (may be frame)
	el2=+el
	MSHTML.IHTMLRect re=el2.getBoundingClientRect
	r.left=re.left; r.top=re.top
	r.right=re.right; r.bottom=re.bottom

 DPI-scaling, zoom
int scaled=DpiIsWindowScaled(sub_Htm.GetDocHwnd(doc))
if(scaled) DpiScale +&r 2
if(_iever>=0x800) sub_Htm.MulDiv sub_Htm.GetZoom(doc) 100 +&r 4

 get pos of container (doc or area)
POINT p
if area.a
	area.Location(p.x p.y)
	if(scaled) DpiScale &p 1
else
	sub_Htm.GetDocXY doc p

 get pos in screen
OffsetRect &r p.x p.y

 note: if DPI 120, in QM web browser control, page is smaller than in IE, but IE COM functions work like same size or bigger. Zoom is 125 in both. Not fixed.
