 /draw_on_control
function hWnd hdc RECT&ru cbParam

__GdiHandle hPen=CreatePen(0 1 0xff0000)
__GdiHandle hBrush=CreateSolidBrush(0xc0E0)
int oldPen=SelectObject(hdc hPen)
int oldBrush=SelectObject(hdc hBrush)
Ellipse hdc 4 4 50 50
SelectObject(hdc oldBrush)
SelectObject(hdc oldPen)

__Font-- f.Create("Comic Sans MS" 12 1)
int oldFont=SelectObject(hdc f)
SetBkMode hdc TRANSPARENT
SetTextColor hdc 0xff
TextOutW hdc 10 14 @"test" 4
SelectObject(hdc oldFont)
