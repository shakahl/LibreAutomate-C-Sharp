function [hWnd]

 Gets window client area DC or screen DC (if hWnd 0), and initializes this variable.
 Calls GetDC.


if(dc) Release
dc=GetDC(hWnd)
