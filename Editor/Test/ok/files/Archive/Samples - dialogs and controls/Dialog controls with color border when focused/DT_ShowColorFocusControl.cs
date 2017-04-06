 /dialog_color_focus_rect
function hDlg

 Call this from dialog procedure whenever focused control changed.

int borderWidth=1

 create static control that will be the focus rectangle
int hr=id(999 hDlg 1)
if(!hr) hr=CreateControl(0 "Static" 0 SS_OWNERDRAW 0 0 0 0 hDlg 999)
ShowWindow hr 0

 get rectangle of focused control and move our focus rect control there
int hf=GetFocus; if(!hf) ret
RECT r; GetWindowRect hf &r; MapWindowPoints 0 hDlg +&r 2
MoveWindow hr r.left r.top r.right-r.left r.bottom-r.top 0

 make our focus rect control hollow, to look like a rectangle with 1 pixel border
OffsetRect &r -r.left -r.top
int r1=CreateRectRgnIndirect(&r)
__GdiHandle r2; InflateRect &r -borderWidth -borderWidth; r2=CreateRectRgnIndirect(&r)
CombineRgn r1 r1 r2 RGN_DIFF
SetWindowRgn hr r1 0

BringWindowToTop hr
ShowWindow hr SW_SHOW
