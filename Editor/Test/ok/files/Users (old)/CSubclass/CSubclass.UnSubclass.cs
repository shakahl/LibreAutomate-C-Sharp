 Call this on WM_DESTROY

if(m_hwnd)
	SubclassWindow(m_hwnd m_wndproc)
	m_hwnd=0
