function x y

 Gets element from point, and initializes this variable.
 Error if fails.

 x, y - point coordinates in screen.

 Added in: QM 2.3.3.


 get doc of element from point. If in frame, it must not be top-level doc. With acc it is easier.
 Acc a=acc(x y 0) ;;any elem in that doc
Acc a.__FromPointDPI(x y) ;;any elem in that doc
MSHTML.IHTMLElement e=htm(a); if(!e) goto gFailed
MSHTML.IHTMLDocument2 doc=e.document
 get doc pos in screen, and element pos in doc
POINT p
int scaled=sub_Htm.GetDocXY(doc p)
x-p.x; y-p.y
if(scaled) DpiScale +&x -1

 get element from point
if(_iever>=0x800) sub_Htm.MulDiv 100 sub_Htm.GetZoom(doc) &x 2
e=doc.elementFromPoint(x y); if(!e) goto gFailed
el=e

err+
	 gFailed
	end ERR_OBJECTGET
