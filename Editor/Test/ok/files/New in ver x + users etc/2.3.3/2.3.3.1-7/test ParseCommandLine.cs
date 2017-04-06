out

str cl="/A /B bbb /c ''ccc ccc'' ''\''ddd ddd\''''"
 str cl="''c:\xx xx\yyy.exe'' /A /B"
ARRAY(str) a
ParseCommandLine(cl a)

int i
for i 0 a.len
	out a[i]
