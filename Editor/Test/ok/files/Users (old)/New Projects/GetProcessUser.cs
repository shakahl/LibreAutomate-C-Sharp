 /
function# str&user pid

 Gets process user name.


SetPrivilege("SeDebugPrivilege")
__Handle hProcess hToken
hProcess=OpenProcess(PROCESS_QUERY_INFORMATION 0 pid) ;;access denied when trying to open other user's process
if(!hProcess) ret

if(!OpenProcessToken(hProcess, TOKEN_QUERY, &hToken)) ret

str st.all(300) sd

if(!GetTokenInformation(hToken, TokenUser, st, 300, &_i) or !_i) ret
TOKEN_USER* pt=st
user.all(300) sd.all(300)
int ulen(300) dlen(300)
if(!LookupAccountSid(0, pt.User.Sid, user, &ulen, sd, &dlen, &_i)) ret
user.fix(ulen)
ret 1
