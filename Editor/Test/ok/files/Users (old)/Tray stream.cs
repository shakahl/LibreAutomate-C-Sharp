str s
rget s "IconStreams" "Software\Microsoft\Windows\CurrentVersion\Explorer\TrayNotify" 0 "" REG_BINARY
 s.ansi
int i
for i 0 s.len
	if(!s[i]) s[i]=32
out s
