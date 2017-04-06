function! hwnd

_i=GetWindowThreadProcessId(hwnd &m_pid)
if(_i=GetCurrentThreadId) ret 1
if(!AttachThreadInput(GetCurrentThreadId _i 1)) ret
m_tid=_i; m_hwnd=hwnd

ret 1

 TODO: detect hung window
