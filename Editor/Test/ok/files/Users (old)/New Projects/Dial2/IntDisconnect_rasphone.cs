 /
function# [$connection]

str s=connection; if(!s.len) RasGetDefConn(s)
str ss.format("-h ''%s''" s)

int r=run("rasphone.exe" ss "" "" 0x400)
if(r or IntConnected2(s)) end "cannot disconnect"
