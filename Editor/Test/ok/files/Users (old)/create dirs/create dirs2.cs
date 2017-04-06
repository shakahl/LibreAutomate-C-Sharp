 This macro also works with QM 2.1.0 - 2.1.4.

str sf sl
sl.getmacro("dirlist")
ARRAY(str) a=sl
int i
for i 0 a.len
	sf=a[i]
	if(!sf.len or sf.beg(" ")) continue
	MkDir sf
