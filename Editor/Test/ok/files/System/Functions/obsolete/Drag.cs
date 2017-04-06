 /
function# hwnd proc param ;;obsolete, use __Drag class

 Simple drag drop.
 Returns when dragging is finished.
 On drop, returns proc's return value. If cancelled, returns 0.

 hwnd - the drag source window.
 proc:
	 Address of a function that will be called while dragging (many times) and when dropped:
	
	 function# button param
	
	 button - mouse button: 0 while dragging, 1 left up, 2 right up.
	 param - param passed to Drag().
	
	 On move, must return cursor handle or: 0 don't change cursor, 1 move, 2 copy, 3 no operation, 4,5 red and blue cross.
	 On button up, can return any value. Drag() returns it.


if(!hwnd) ret
MSG m; int R; int mb

SetCapture(hwnd)
rep
	if(GetCapture!=hwnd || GetMessage(&m 0 0 0)<1) break
	
	mb=0
	sel(m.message)
		case [WM_KEYDOWN,WM_SYSKEYDOWN]
		if(m.wParam==VK_ESCAPE) ReleaseCapture()
		continue
		
		case WM_MOUSEMOVE
		R=call(proc 0 param)
		if R
			if(R>=1 and R<=5) R=sub_to.LoadCursor(R)
			SetCursor(R)
		continue
		
		case WM_LBUTTONUP mb=MK_LBUTTON
		case WM_RBUTTONUP mb=MK_RBUTTON
	
	if(mb)
		ReleaseCapture
		ret call(proc mb param)
	
	if(m.message>=WM_MOUSEFIRST && m.message<=WM_MOUSELAST) continue
	DispatchMessage(&m)

if(m.message==WM_QUIT) PostQuitMessage(m.wParam)
