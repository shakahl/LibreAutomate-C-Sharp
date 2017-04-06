__Handle hToken
str st

if(!OpenProcessToken(GetCurrentProcess TOKEN_QUERY &hToken)) ret
if(!GetTokenInformation(hToken TokenOwner st.all(300) 300 &_i) or !_i) ret

SID* p; memcpy &p st 4
lpstr k
if(!ConvertSidToStringSid(p &k)) ret
out k
LocalFree k

 if QM Admin: S-1-5-32-544
 if QM User:  S-1-5-21-364929558-101999248-426651109-1001
