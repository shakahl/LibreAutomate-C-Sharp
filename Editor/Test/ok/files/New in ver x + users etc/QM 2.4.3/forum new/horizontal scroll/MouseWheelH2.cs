 /
function nticks [hwnd] [flags] ;;nticks: >0 to right, <0 to left.   flags: 1 window from mouse, 2 pages, 4 full

 Mouse horizontal wheel.

 nticks - number of wheel ticks to scroll to the right (positive) or left (negative).
 hwnd - handle of a child window to scroll. If not used, scrolls the focused child window.

 REMARKS
 Sends WM_HSCROLL message. Does not work with windows that don't support the message.
 In some windows, scrollbars are separate controls, usually of class "ScrollBar". Then this function probably will not work.


if(!hwnd) hwnd=iif(flags&1 child(mouse) child)
int m; if(nticks>0) m=SB_LINERIGHT; else m=SB_LINELEFT; nticks=-nticks
if(flags&2) m+2; else if(flags&4) m+6
rep(nticks) SendMessage hwnd WM_HSCROLL m 0
