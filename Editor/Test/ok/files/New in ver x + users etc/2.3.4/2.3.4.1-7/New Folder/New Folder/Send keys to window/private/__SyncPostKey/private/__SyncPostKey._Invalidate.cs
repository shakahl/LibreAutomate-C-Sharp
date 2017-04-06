function!

 Invalidates 1 pixel of m_hwnd client area.
 Returns 1. If client area is not in screen, returns 0; then redrawwindow will not work, ie the update rect will be always empty; also if minimized or hidden.


if(!IsWindowVisible(m_hwnd)) ret

 Find top-left 1 pixel of m_hwnd client area that is in screen.
 Simply invalidating top-left pixel does not work if it is not in screen. Invalidating whole client area is slow and flickers.
MONITORINFO m.cbSize=sizeof(m)
if(!GetMonitorInfo(MonitorFromWindow(m_hwnd 0) &m)) ret
RECT rc r=m.rcMonitor
MapWindowPoints 0 m_hwnd +&r 2
GetClientRect m_hwnd &rc
if(!IntersectRect(&r &r &rc)) ret ;;info: this also detects minimized window, ie empty client rect
r.right=r.left+1; r.bottom=r.top+1

ret 0!RedrawWindow(m_hwnd &r 0 RDW_INVALIDATE|RDW_NOCHILDREN) ;;invalidates and posts WM_PAINT, which must validate

 TODO: test with RTL
