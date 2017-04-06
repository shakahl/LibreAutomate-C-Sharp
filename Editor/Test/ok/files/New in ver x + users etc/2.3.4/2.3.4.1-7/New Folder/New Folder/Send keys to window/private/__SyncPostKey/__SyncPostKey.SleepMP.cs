if(!m_pam) GetProcessAffinityMask(GetCurrentProcess &m_pam &_i)
if(m_pam<2) Sleep 0; ret ;;1 CPU

int i(1) tam
rep
	if m_pam&i
		_i=SetThreadAffinityMask(GetCurrentThread i)
		if(!tam) tam=_i
		Sleep 0
	i<<1; if(!i) break

if(tam) SetThreadAffinityMask(GetCurrentThread tam)
