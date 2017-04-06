function# [^waitMax] [&getHwnd]

 Waits until a windows added by AddWin etc is created. However it may be invisible or not active at that time.
 Returns 1-based index of the window.
 Error if parent window does not exist when called or destroyed while waiting.

 waitMax - max number of seconds to wait. 0 is infinite. Error when expires.
 getHwnd - receives window handle.


opt waitmsg -1
opt hidden -1
ret Wait(0 waitMax getHwnd)
err end _error
