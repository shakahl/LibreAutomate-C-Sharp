dll user32 #GetCaretPos POINT*lpPoint
dll user32 #SetCaretPos x y


POINT xy
 int& x
SetCaretPos(190 119)
3
GetCaretPos(&xy)
out xy.x
out xy.y

 error: "This program has performed an illegal operation and will be shut down."