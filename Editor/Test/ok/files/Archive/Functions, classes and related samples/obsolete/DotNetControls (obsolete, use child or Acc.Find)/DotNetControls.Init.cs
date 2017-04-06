function hwndParent

 Call this function before calling other functions.
 Fails if the process belongs to another user.
 On Vista, fails if QM is running as User and the process has higher integrity level.
 Error if fails.

 hwndParent - parent window handle.


m_pm.Alloc(hwndParent 4096)
err end _error

m_msg=RegisterWindowMessage("WM_GETCONTROLNAME")
m_hwnd=hwndParent
