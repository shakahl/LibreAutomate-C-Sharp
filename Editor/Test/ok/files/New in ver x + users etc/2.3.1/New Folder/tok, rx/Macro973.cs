out

str subject="''pns1'', ''pn, s2'': goto xxx"

 at first separate part after :
int i=findcr(subject ':')
str afterColon
if(i>=0) afterColon.get(subject i+1); afterColon.ltrim; subject.fix(i)

out subject
out afterColon
out "--------"

 then tok
ARRAY(str) a a2
tok subject a -1 "'', " 4 a2
for i 0 a.len
	out "'%s'    '%s'" a[i] a2[i]
