Acc a.FromMouse
int x y w h
a.Location(x y w h) ;;get screen coord of the object
RECT r; SetRect &r x y x+w y+h ;;convert to RECT

OnScreenRect 1 r; 0.5; OnScreenRect 2 r ;;remove this if don't need to show the rectangle

int hwnd=GetAncestor(a.Hwnd 2) ;;get top-level parent window
MapWindowPoints 0 hwnd +&r 2 ;;screen to client

out F"SetRect &a[91]] {r.left} {r.top} {r.right} {r.bottom}"

err+
