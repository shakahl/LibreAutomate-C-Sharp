 /
function x y hwnd [flags] [button] ;;flags: 1 client area.  button: 0,1 left, 2 right, 3 middle, 4 left double

 Sends mouse click message to the window without moving the mouse.
 This works not with all windows.
 The window can be inactive. However the function activates some windows.
 In most cases works synchronously, ie waits while the window is processing the messages.
 No errors, no return value, no autodelay.
 To insert code with this function, use the Mouse dialog and then replace 'lef' to 'SendClickMessage'.

 x y - mouse coordinates relative to hwnd. If flag 1 used - relative to client area of hwnd.
 hwnd - handle of window or child window.

 EXAMPLE
 SendClickMessage 17 15 id(129 win("Calculator" "SciCalc"))


if(!(flags&1)) RECT r; GetWindowRect(hwnd &r); x+=r.left; y+=r.top; ScreenToClient(hwnd +&x) ;;window to client
int w1=child(x y hwnd 8); err
if(w1)
	MapWindowPoints(hwnd w1 +&x 1)
	hwnd=w1

int m1 m2 mk m11
sel button
	case 2 m1=WM_RBUTTONDOWN; m2=WM_RBUTTONUP; mk=MK_RBUTTON
	case 3 m1=WM_MBUTTONDOWN; m2=WM_MBUTTONUP; mk=MK_MBUTTON
	case else m1=WM_LBUTTONDOWN; m2=WM_LBUTTONUP; mk=MK_LBUTTON
m11=m1

int xy=y<<16|x
 g1
SendMessage(hwnd WM_SETCURSOR hwnd m1<<16|HTCLIENT)
SendMessage(hwnd m11 mk xy)

SendMessage(hwnd WM_SETCURSOR hwnd m2<<16|HTCLIENT)
SendMessage(hwnd m2 0 xy)

if(button=4) button=0; m11=WM_LBUTTONDBLCLK; goto g1
