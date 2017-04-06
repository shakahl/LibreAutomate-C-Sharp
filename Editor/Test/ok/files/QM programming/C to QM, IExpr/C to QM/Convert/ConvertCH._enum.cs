 /CtoQM
function# str&sn str&sm

 Converts enum.

ARRAY(str) a
int i j db
str s1 s2 expr s1prev

if(sn.len and sn!="#") AddToMap(m_mtd sn "#" "" 1)

tok sm a -1 ","
for(i 0 a.len)
	if(tok(a[i] &s1 2 "= " 2)<2)
		if(expr.len) s2.from(expr "+" j+1)
		else s2=j
	else
		if(findrx(s2 "^-?(\d+|0x[\dA-F]+)L?$" 0 1)=0)
			if(s2.end("L") or s2.end("U")) s2.fix(s2.len-1)
			j=val(s2)
			expr.fix(0)
		else if(s2.beg(s1prev) and !expr.len and (findrx(s2+s1prev.len "^ *\+ *1$")=0 or !s2[s1prev.len]))
			 db=1
			if(!s2[s1prev.len]) j-1 ;;same
			s2=j
			expr.fix(0)
		else
			 db=1
			j=0
			expr=s2
	if(findrx(s1 "^[A-Z_]\w*$" 0 1)) ret 41 ;;name expanded as macro due to wrong file order
	j+1
	s1prev=s1
	AddToMap(m_mc s1 s2)
	
	 if(db) out "%s %s" s1 s2

if(db) out sm

 Bug: some GDI+ enum InterpolationMode members calculated incorrectly.
