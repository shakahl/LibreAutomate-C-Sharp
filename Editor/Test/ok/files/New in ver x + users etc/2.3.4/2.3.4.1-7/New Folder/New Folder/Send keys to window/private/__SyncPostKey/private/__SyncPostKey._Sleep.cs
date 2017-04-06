if(!m_pam) GetProcessAffinityMask(GetCurrentProcess &m_pam &_i)
if m_pam>1
	if(!m_am) m_am=0x80000000
	rep() m_am=_lrotl(m_am 1); if(m_pam&m_am) break
	_i=SetThreadAffinityMask(GetCurrentThread m_am)
	if(!m_tam) m_tam=_i

Sleep 0
 SendMessageTimeout m_hwnd 0 0 0 0 100 &_i
