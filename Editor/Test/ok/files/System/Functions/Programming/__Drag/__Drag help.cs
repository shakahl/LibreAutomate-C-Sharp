 Manages non-OLE drag-drop.
 At first call Init() to set drag-drop parameters. A window handle is required.
 Then call Next() in a loop (rep). End the loop when Next() returns 0.

 Next() sets these member variables that you can use:
   dropped - when Next() returns 0 to end the drag loop, it sets this variable to: 0 cancelled, 1 left button up, 2 right button up.
   p (POINT) - mouse position in hwnd client area.
   mk - modifier key flags: MK_SHIFT, MK_CONTROL.
   m (MSG) - current message retrieved by GetMessage.
 In the loop you can set member variables:
   cursor - cursor to show. Can be cursor handle, or: 1 move, 2 copy, 3 no operation, 4 red cross, 5 blue cross, 6 link.

 EXAMPLE
	case WM_LBUTTONDOWN goto gDrag
 ...
 gDrag
__Drag x.Init(hDlg 1)
rep
	if(!x.Next) break
	x.cursor=iif(x.mk&MK_CONTROL 2 1)
	 ...
if(!x.dropped) ret ;;eg user pressed Esc
out "dropped"
ret
