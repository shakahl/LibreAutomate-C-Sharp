 does not work
if(_winnt<6 or !IsUserAdmin) ret
 ret
__Handle ht1 ht2
if(!OpenProcessToken(GetCurrentProcess MAXIMUM_ALLOWED &ht1)) ret
if(!DuplicateTokenEx(ht1 MAXIMUM_ALLOWED 0 SecurityImpersonation TokenImpersonation &ht2)) ret
out ht2
SID* ps
dll advapi32 #ConvertStringSidToSid $StringSid !**Sid
if(!ConvertStringSidToSid("S-1-16-8192" &ps)) ret
TOKEN_MANDATORY_LABEL til.Label.Attributes=SE_GROUP_INTEGRITY; til.Sid=ps
if(!SetTokenInformation(ht2 TokenIntegrityLevel &til sizeof(til)+GetLengthSid(ps))) ret
int uiAccess; if(!SetTokenInformation(ht2 TokenUIAccess &uiAccess 4)) ret
LocalFree ps
m_imp=ImpersonateLoggedOnUser(ht2)
out m_imp
