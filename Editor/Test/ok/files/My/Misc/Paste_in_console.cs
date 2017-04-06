 /
function# iid FILTER&f

if(f.hwnd and wintest(f.hwnd "" "ConsoleWindowClass"))
	PostMessage f.hwnd WM_SYSCOMMAND 65521 0
	ret -1
ret -2
