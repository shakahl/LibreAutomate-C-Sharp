function hwnd hdc cx cy OSDLINE&a

 screen to client
POINT p p2
memcpy &p &a.x sizeof(POINT)*2
MapWindowPoints 0 hwnd &p 2

int hpen oldpen
hpen=CreatePen(0 a.linewidth a.color); oldpen=SelectObject(hdc hpen)
MoveToEx hdc p.x p.y 0 ;;set current position
LineTo hdc p2.x p2.y
DeleteObject SelectObject(hdc oldpen)
