function! hwnd
if(m_hwnd and hwnd=m_hwnd) ret 1
int pid
if(!GetWindowThreadProcessId(hwnd &pid)) ret
if(m_pid and pid=m_pid) ret 1
m_pm.Alloc(pid 4096 1)
err ret
m_hwnd=hwnd
m_pid=pid
ret 1
