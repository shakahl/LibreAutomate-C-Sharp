 /
function hDlg $buttons

 Adds down-arrow to one or more buttons to indicate that they open a menu etc.
 Subclasses and draws at the right side. The button can have text.

 buttons - space-separated list of button ids.

__IdStringParser p.Parse(hDlg buttons)

for(_i 0 p.a.len) SetWindowSubclass(p.a[_i].hwnd &sub.WndProc 0 0)


#sub WndProc
function# hWnd message wParam lParam uIdSubclass dwRefData

int R=DefSubclassProc(hWnd message wParam lParam)
sel message
	case WM_NCDESTROY RemoveWindowSubclass(hWnd &sub.WndProc 0)
	case WM_PAINT goto gPaint
ret R

 gPaint
__Font-- f.Create("Marlett" 10)
int hdc=GetDC(hWnd)
int of=SelectObject(hdc f)
RECT r; GetClientRect hWnd &r; InflateRect &r -1 0
SetBkMode hdc TRANSPARENT
DrawTextW hdc L"6" 1 &r DT_RIGHT|DT_VCENTER|DT_SINGLELINE
SelectObject(hdc of)
ReleaseDC(hWnd hdc)

ret R
