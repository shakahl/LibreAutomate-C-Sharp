function$ pid [flags] ;;flags: 1 with domain

 Gets process user name.
 QM must be running as admin.

 pid - process id. Use ProcessNameToId to get id from name.


__Handle hProcess hToken
str st sd

this.all
SetPrivilege("SeDebugPrivilege")
hProcess=OpenProcess(PROCESS_QUERY_INFORMATION 0 pid); if(!hProcess) ret
if(!OpenProcessToken(hProcess TOKEN_QUERY &hToken)) ret
if(!GetTokenInformation(hToken TokenUser st.all(300) 300 &_i) or !_i) ret

TOKEN_USER* pt=st
this.all(300); sd.all(300)
int ul(300) dl(300)
if(!LookupAccountSid(0 pt.User.Sid this &ul sd &dl &_i)) ret
this.fix(ul)
if(flags&1) this.from(sd.lpstr "/" this)
ret this
