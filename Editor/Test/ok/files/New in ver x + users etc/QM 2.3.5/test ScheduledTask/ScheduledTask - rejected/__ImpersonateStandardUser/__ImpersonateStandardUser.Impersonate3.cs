 does not work
if(_winnt<6 or !IsUserAdmin) ret
 ret
__Handle ht1 ht2 ht3
if(!OpenProcessToken(GetCurrentProcess MAXIMUM_ALLOWED &ht1)) ret
if(!GetTokenInformation(ht1 TokenLinkedToken &ht2 4 &_i)) ret
if(!DuplicateTokenEx(ht2 MAXIMUM_ALLOWED 0 SecurityImpersonation TokenImpersonation &ht3)) ret
out ht3
m_imp=ImpersonateLoggedOnUser(ht3)
out m_imp
