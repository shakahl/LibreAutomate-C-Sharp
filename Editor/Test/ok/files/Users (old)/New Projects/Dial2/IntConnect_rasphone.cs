 /
function# [$connection]

str s=connection; if(!s.len) RasGetDefConn(s)
str ss.format("-d ''%s''" s)

int i r=run("rasphone.exe" ss "" "" 0x400)
if(!r and IntIsConnected(s)) ret 1
