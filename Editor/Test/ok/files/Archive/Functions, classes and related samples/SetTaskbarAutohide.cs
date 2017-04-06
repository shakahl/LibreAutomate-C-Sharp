 /
function state

 Hides (sets "auto hide") or unhides (sets "always on top") taskbar.

 EXAMPLE
  Trigger: window activated. Hides taskbar when the window activated, and unhides when deactivated or closed.
 int hwnd=val(_command)
 SetTaskbarAutohide 1
 rep() 0.5; if(win!=hwnd) SetTaskbarAutohide 0; break


APPBARDATA ABD.cbSize = sizeof(ABD)
SHAppBarMessage ABM_GETSTATE, &ABD
if state
	ABD.lParam = ABS_AUTOHIDE
else
	ABD.lParam = ABS_ALWAYSONTOP
SHAppBarMessage ABM_SETSTATE, &ABD
