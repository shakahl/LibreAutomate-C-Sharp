str s="super documentation about super scripting.pdf"
str name.getfilename(s) ext=s+name.len
ARRAY(str) a; int i; str ss
if(tok(name a)>1)
	for(i 0 a.len)
		str& r=a[i]
		if(r.len>3) r.fix(3); r.rtrim("aeiyou"); r[0]=toupper(r[0])
		ss+r
ss+ext
out ss
