 /
function hwnd hdc cx cy param

int hpen oldpen

hpen=CreatePen(0 2 0xff0000); oldpen=SelectObject(hdc hpen)
Ellipse hdc 0 0 cx cy
DeleteObject SelectObject(hdc oldpen)
