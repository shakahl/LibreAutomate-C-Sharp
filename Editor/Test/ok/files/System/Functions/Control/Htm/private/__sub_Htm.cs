#sub GetDocXY
function# MSHTML.IHTMLDocument2&doc [POINT&p] [RECT&r]

 Gets screen position or rect of doc.
 Use either p or r.
 Returns 1 if the window is DPI-scaled, else 0.

opt noerrorshere 1

IOleWindow iw=+doc
Acc a=acc(iw)
if(&p) a.Location(p.x p.y)
else a.Location(r.left r.top r.right r.bottom); r.right+r.left; r.bottom+r.top

 IE bug fix. If gets AO from an HTML object, Location gets logical coord.
iw.GetWindow(_i)
_i=DpiIsWindowScaled(_i)
if(_i) if(&p) DpiScale &p 1; else DpiScale +&r 2
ret _i

 Tried MSHTML, but it does not always work (with high DPI, zoom, iframes etc). And much slower.
 MSHTML.IHTMLWindow2 w2=doc.parentWindow
 MSHTML.IHTMLWindow3 w3=+w2
 int k1(ScreenWidth) k2(w2.screen.width)
 p.x=MulDiv(w3.screenLeft k1 k2)
 p.y=MulDiv(w3.screenTop k1 k2)

 Gets rect with border width. Then element rectangles are shifted up-left etc.
 Can detect border, but it is difficult, unreliable, and makes 5-10 times slower.


#sub GetDocHwnd
function# MSHTML.IHTMLDocument2&doc

IOleWindow ow=+doc
ow.GetWindow(_i)
ret _i
err+


#sub GetZoom
function# MSHTML.IHTMLDocument2&doc

if(_iever<0x800) ret ;;although optical zoom is in IE7 too, but in IE7 don't need to convert coordinates

IOleCommandTarget ct=+doc ;;IE8
VARIANT zoom
ct.Exec(0 OLECMDID_OPTICAL_ZOOM OLECMDEXECOPT_DONTPROMPTUSER 0 &zoom)

err+ zoom=100
ret zoom


#sub MulDiv
function mul div *p n
 MulDiv for multiple values.

if(mul=div or !mul or !div) ret
for(n n 0 -1) *p=MulDiv(*p mul div); p+4
