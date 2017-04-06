out
int w1=win("Untitled - Notepad" "Notepad")
 int w1=win("Dialog1" "#32770")
int w2=win("Dialog2" "#32770")
 Zorder w1 HWND_TOP
Zorder w1 HWND_TOPMOST
 Zorder w2 HWND_TOPMOST
 outx GetWinStyle(w1 1)&WS_EX_TOPMOST
 Zorder w1 HWND_TOP
 Zorder w2 HWND_TOPMOST
 act _hwndqm
1
int ws=win("" "Shell_TrayWnd")
 act ws

	 int h=CreateWindowEx(WS_EX_TOOLWINDOW +32770 0 WS_POPUP 0 0 0 0 0 0 _hinst 0)
	 act h; err

 Zorder w1 HWND_NOTOPMOST
 SetWindowPos w1 HWND_NOTOPMOST 0 0 0 0 SWP_NOSIZE|SWP_NOMOVE|SWP_NOACTIVATE
 Zorder w2 HWND_NOTOPMOST
Zorder w1 HWND_BOTTOM
 rep(2)0.3;SetWindowPos(w1 HWND_BOTTOM 0 0 0 0 SWP_NOSIZE|SWP_NOMOVE|SWP_NOACTIVATE)
 1
 Zorder w1 _hwndqm
 Zorder w1 ws;; SWP_NOACTIVATE

	 DestroyWindow(h)

 SetWindowPos w1 _hwndqm 0 0 0 0 SWP_NOSIZE|SWP_NOMOVE|SWP_NOACTIVATE|SWP_NOOWNERZORDER
 rep(1) out SetWindowPos(w1 _hwndqm 0 0 0 0 SWP_NOSIZE|SWP_NOMOVE|SWP_NOACTIVATE)
