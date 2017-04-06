Acc c=acc("" "LISTITEM" win("" "CabinetWClass") "SysListView32" "" 0x1001)
str s=c.Description
out s
ARRAY(str) a; int i
if(findrx(s "^.+?: (.+?)[,;] .+?: (.+?)[,;] .+?: (.+)" 0 0 a)<0) ret
for i 0 a.len
	out a[i]

 info: the separator is as set in Control Panel Regional. Tested on Windows 7.
