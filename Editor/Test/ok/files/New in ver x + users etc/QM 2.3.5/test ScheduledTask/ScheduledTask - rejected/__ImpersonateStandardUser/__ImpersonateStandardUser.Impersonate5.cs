 does not work
if(_winnt<6 or !IsUserAdmin) ret
 ret
__Handle ht1 ht2
if(!OpenProcessToken(GetCurrentProcess MAXIMUM_ALLOWED &ht1)) ret
if(!CreateRestrictedToken(ht1 LUA_TOKEN 0 0 0 0 0 0 &ht2)) ret
out ht2
m_imp=ImpersonateLoggedOnUser(ht2)
