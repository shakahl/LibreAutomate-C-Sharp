 /
function# iid QMITEM&q level IStringMap&m

str& t=q.name
rep 2
	ARRAY(str) a; int i
	if(!tok(t a)) ret
	for i 0 a.len
		str& s=a[i]
		if(s.len<=2 or isdigit(s[0])) continue
		m.Add(s)
	&t=q.text
