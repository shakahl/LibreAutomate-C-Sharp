function# [^waitMax]

 Waits until a windows added by AddWin etc is destroyed.
 Returns 1-based index of the window.
 Succeeds if parent window does not exist when called or destroyed while waiting.

 waitMax - max number of seconds to wait. 0 is infinite. Error when expires.


opt waitmsg -1
opt hidden -1
ret Wait(1 waitMax 0)
err end _error
