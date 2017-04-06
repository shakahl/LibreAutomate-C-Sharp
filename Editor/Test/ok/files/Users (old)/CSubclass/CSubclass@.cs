if(m_hwnd)
	if(m_flags&1) DestroyWindow(m_hwnd)
	else SubclassWindow(m_hwnd m_wndproc)
