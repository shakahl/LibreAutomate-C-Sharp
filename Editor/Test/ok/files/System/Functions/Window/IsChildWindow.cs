 /
function! hwnd

 Returns 1 if window is child window (control), 0 if top-level window.

 hwnd - window handle.


ret RealGetParent(hwnd)!0
