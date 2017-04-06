function# [^waitMax] [&getHwnd]

 Waits until a windows added by AddWin etc is visible.
 Returns 1-based index of the window.
 The window may not exist when called. The functions waits until it is created and visible.
 Error if parent window does not exist when called or destroyed while waiting.

 waitMax - max number of seconds to wait. 0 is infinite. Error when expires.
 getHwnd - receives window handle.


opt waitmsg -1
opt hidden -1
ret Wait(3 waitMax getHwnd)
err end _error
