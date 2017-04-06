type WINDOWINFO hwnd hwndparent hwndtopparent hwndowner id style exstyle ~text ~class ~program ~programpath RECT'rectangle
class WindowInfo hwnd

 WINDOWINFO w
str s
WindowInfo w.hwnd=child(mouse)
if(!w.hwnd) w.hwnd=win(mouse)

w.GetAllInfo(0 s)
out s
