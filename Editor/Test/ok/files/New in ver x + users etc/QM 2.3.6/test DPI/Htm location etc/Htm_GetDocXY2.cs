 /Macro1580
function# MSHTML.IHTMLDocument2&doc [POINT&p] [RECT&r]

 Gets screen position or rect of doc.
 Use either p or r.
 Returns 1 if the window is DPI-scaled, else 0.

opt noerrorshere 1

IOleWindow iw=+doc
Acc a=acc(iw)
if(&p) a.Location(p.x p.y)
else a.Location(r.left r.top r.right r.bottom); r.right+r.left; r.bottom+r.top

 MSHTML.IHTMLElement2 body=+doc.body
 MSHTML.IHTMLRect hr=body.getBoundingClientRect
 RECT rr.left=hr.left; rr.top=hr.top; rr.right=hr.right; rr.bottom=hr.bottom
 zRECT r
 zRECT rr

 MSHTML.IHTMLWindow5 w=doc.parentWindow
 w.
 out body.clientTop
 out body.clientHeight
MSHTML.IHTMLDocument3 d3=+doc
MSHTML.IHTMLElement2 e2=+d3.documentElement
 out e2.clientHeight
 out e2.clientTop
 out MulDiv(e2.clientHeight 120 96)
MSHTML.IHTMLRect hr=e2.getBoundingClientRect
RECT rr.left=hr.left; rr.top=hr.top; rr.right=hr.right; rr.bottom=hr.bottom
Htm_MulDiv 120 96 +&rr 4
 zRECT rr


 doc.styleSheets.
 MSHTML.IHTMLDocument4 d4=+doc
 d4.



 IE bug fix. If gets AO from an HTML object, Location gets logical coord.
iw.GetWindow(_i)
_i=DpiIsWindowScaled(_i)
if(_i) if(&p) DpiScalePoint &p; else DpiScaleRect &r
ret _i

  Tried MSHTML, but it does not always work (with high DPI, zoom, iframes etc). And much slower.
 MSHTML.IHTMLWindow2 w2=doc.parentWindow
 MSHTML.IHTMLWindow3 w3=+w2
 int k1(ScreenWidth) k2(w2.screen.width)
 p.x=MulDiv(w3.screenLeft k1 k2)
 p.y=MulDiv(w3.screenTop k1 k2)
