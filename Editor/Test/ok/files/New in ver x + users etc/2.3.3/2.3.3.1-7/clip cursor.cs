RECT r

int width(150) height(150) ;;change these values
r.right=ScreenWidth; r.bottom=ScreenHeight
InflateRect &r -r.right/2+(width/2) -r.bottom/2+(height/2)

 the above code calculates rectangle of specified width/height in screen center. Alternatively use code like this:
 r.left=100
 r.top=100
 r.right=r.left+150
 r.bottom=r.top+150

 _____________________________

RECT rr
if(GetClipCursor(&rr) and !memcmp(&r &rr sizeof(r))) ClipCursor 0
else ClipCursor &r
