 /
function# str&s [noerr]

if(rget(s "DefaultInternet" "Software\Microsoft\Ras Autodial\Default")) ret 1
if(rget(s "DefaultInternet" "Software\Microsoft\Ras Autodial\Default" HKEY_LOCAL_MACHINE)) ret 1

ARRAY(str) a
int r=RasGetEntries(a)
if(!r and a.len) s=a[0]; ret 1
if(!noerr) RasError r "cannot get default connection"
