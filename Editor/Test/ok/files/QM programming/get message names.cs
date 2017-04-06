 Macro "WM messages" should contain messages where each line is MESSAGENAME MESSAGEVALUE.
 This macro parses it and creates sorted C array of message names.

out
str s.getmacro("WM messages")
ARRAY(str) a
findrx(s "^(WM_\w+) (\w+)" 0 4|8 a)
int i j
 for i 0 a.len
	 out "%s %i" a[1 i] val(a[2 i])

s.fix(0)
lpstr wm
for j 0 WM_USER+1
	for i 0 a.len
		if(val(a[2 i])=j) break
	if(i=a.len) wm=0
	else wm=a[1 i]
	 s.formata("%i %s[]" j wm)
	s.formata("''%s''," wm)

s.fix(s.len-1)
s-"static LPSTR a[ ]={"
s+"};"
out s
