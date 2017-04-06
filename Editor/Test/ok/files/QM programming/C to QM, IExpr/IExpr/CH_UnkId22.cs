function# $ID long&r flags IStringMap&pr
out ID

str s; int u
if(flags&1)
	sel ID 2
		case "defined(*)"
		s.get(ID 8 len(ID)-9); s.trim
		 out s
		if(pr.Get(s)) r=1; else r=0
		ret 1
		
		case "sizeof*(*)"
		if(findrx(ID "^sizeof *\( *(L?''.*)'' *\)" 0 0 s 1)<0) end "sizeof() error" 1
		UnescapeStringC s
		r=s.len; if(s[0]='L') r=r-1*2
		ret 1
		
		case "Function40(*)"
		r=&Function40
		 out r
		ret 257
else
	r=100
	ret 1
 ret 1
