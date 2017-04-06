dll ntdll #NtQueryTimerResolution *maxi *mini *cur

int mini maxi cur
rep
	1
	if(NtQueryTimerResolution(&maxi &mini &cur)) out "failed"; continue
	 out F"{mini} {maxi} {cur}"; ret
	if cur<150000
		_s.timeformat("{TT}")
		out F"{cur}  {_s}"
