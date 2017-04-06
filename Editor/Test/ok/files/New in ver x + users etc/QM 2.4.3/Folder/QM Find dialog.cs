Results -> bottom :sub.MoveResults 1
Results -> right :sub.MoveResults 0


#sub MoveResults
function bottom

int w=GetToolbarOwner(TriggerWindow)
RECT r
int c=id(3006 w) ;;list
int t=id(3084 w) ;;push button '6'
GetWindowRect t &r; ScreenToClient w +&r.right
 int W H; GetWinXY c 0 0 W H

int xNC yNC; RECT rw rc; GetWindowRect w &rw; GetClientRect w &rc; xNC=rw.right-rw.left-rc.right; yNC=rw.bottom-rw.top-rc.bottom

if bottom
	mov 0 r.bottom+8 c
	siz r.right+8+xNC r.bottom*4+yNC w
else
	mov r.right+8 0 c
	siz r.right*2+xNC r.bottom+4+yNC w
EnsureWindowInScreen w
SendMessage w WM_SIZE 0 0
