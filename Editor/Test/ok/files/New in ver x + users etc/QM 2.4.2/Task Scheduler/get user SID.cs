__Handle hToken
str st

if(!OpenProcessToken(GetCurrentProcess TOKEN_QUERY &hToken)) ret
if(!GetTokenInformation(hToken TokenUser st.all(300) 300 &_i) or !_i) ret

TOKEN_USER* tu=st
lpstr k
if(!ConvertSidToStringSid(tu.User.Sid &k)) ret
out k
LocalFree k

 if QM Admin: S-1-5-21-364929558-101999248-426651109-1001
 if QM User:  S-1-5-21-364929558-101999248-426651109-1001
