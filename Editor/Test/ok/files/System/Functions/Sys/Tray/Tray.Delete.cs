
 Deletes tray icon.


if(m_flags&1)
	m_flags~1
	__Shell_NotifyIconW(NIM_DELETE &nd)
	m_a.redim
__m_func=0
m_hwnd=0
