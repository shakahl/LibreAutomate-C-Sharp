 /
function# hwnd [ex]

 Gets window style or extended style.
 The return value is <google "site:microsoft.com window styles WS_VISIBLE WS_CHILD">window style</google> or <google "site:microsoft.com extended window styles WS_EX_TOOLWINDOW">extended window style</google> flags.

 hwnd - window handle. Can be top-level or child window.
 ex - if nonzero, gets extended style.

 EXAMPLES
  Get "Notepad" window style:
 int h = win("Notepad")
 int st = GetWinStyle(h)
 if(st & WS_MAXIMIZE) out "Notepad is maximized"

  Get extended style:
 int exst = GetWinStyle(h 1)
 if(exst & WS_EX_TOPMOST) out "Notepad is ''Always on Top''"


ret GetWindowLong(hwnd iif(ex GWL_EXSTYLE GWL_STYLE))
