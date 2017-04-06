int w=wait(3 WV win("Quick Macros - automation software for Windows. Macro program. Keyboard, mouse, record, toolbar - Windows Internet Explorer" "IEFrame"))
Htm e=htm("A" "Purchase " "" w "0" 8 0x21 3)

int x y cx cy
e.Location(x y cx cy)
out "%i %i %i %i" x y cx cy
RECT r rr
e.GetRect(r)
zRECT r
out HtmlLocation(e rr)
zRECT rr

 e.ClickAsync
 e.Mouse(0)

outw e.Hwnd
