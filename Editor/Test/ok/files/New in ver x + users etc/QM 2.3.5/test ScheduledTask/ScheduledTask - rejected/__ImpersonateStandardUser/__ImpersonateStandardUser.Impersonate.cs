 works, but if QM is "run as", gets wrong token
if(_winnt<6 or !IsUserAdmin) ret
 ret
if(!GetWindowThreadProcessId(GetShellWindow &_i)) ret
__Handle ht hp=OpenProcess(PROCESS_QUERY_INFORMATION 0 _i)
if(!OpenProcessToken(hp TOKEN_QUERY|TOKEN_DUPLICATE &ht)) ret
m_imp=ImpersonateLoggedOnUser(ht)
