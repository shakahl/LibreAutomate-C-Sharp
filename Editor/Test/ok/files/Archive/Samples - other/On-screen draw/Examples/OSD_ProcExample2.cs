 /OSD example2
function hwnd hdc cx cy OSDCOLORANDBORDER&a

int hpen oldpen

 create/select pen and draw rectangle
hpen=CreatePen(0 10 a.color); oldpen=SelectObject(hdc hpen)
Rectangle hdc a.border a.border cx-a.border cy-a.border ;;simple
DeleteObject SelectObject(hdc oldpen)
