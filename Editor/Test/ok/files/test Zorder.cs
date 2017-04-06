 int w=win("Document1 - Microsoft Word" "OpusApp")
 w=GetTopWindow(0)
 outw w
 outx GetWinStyle(w 1)

int w=win("Untitled - Notepad" "Notepad")
 Zorder w HWND_TOP
 Zorder w HWND_TOP SWP_NOACTIVATE

 int h=CreateWindowEx(WS_EX_TOOLWINDOW +32770 0 WS_POPUP 0 0 0 0 0 0 _hinst 0)
 act h; err
 out SetWindowPos(w HWND_TOP 0 0 0 0 SWP_NOSIZE|SWP_NOMOVE)
 DestroyWindow(h)

int Word=win("Microsoft Word")
act Word
0.5

out SetWindowPos(w HWND_NOTOPMOST 0 0 0 0 SWP_NOSIZE|SWP_NOMOVE|SWP_NOACTIVATE)
 int wa=win
 out SetWindowPos(w wa 0 0 0 0 SWP_NOSIZE|SWP_NOMOVE|SWP_NOACTIVATE)
 out SetWindowPos(wa w 0 0 0 0 SWP_NOSIZE|SWP_NOMOVE|SWP_NOACTIVATE)

 out SetWindowPos(Word HWND_BOTTOM 0 0 0 0 SWP_NOSIZE|SWP_NOMOVE|SWP_NOACTIVATE)
