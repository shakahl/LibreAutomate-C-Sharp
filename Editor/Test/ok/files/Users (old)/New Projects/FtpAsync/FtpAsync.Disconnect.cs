if(m_fd)
	if(m_fd.hfind) InternetCloseHandle(m_fd.hfind)
	m_fd._delete
if(m_hi) InternetCloseHandle(m_hi); m_hi=0

InternetSetStatusCallback(m_hitop 0)
