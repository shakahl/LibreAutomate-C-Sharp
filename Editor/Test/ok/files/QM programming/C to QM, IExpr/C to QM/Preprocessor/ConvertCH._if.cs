 /CtoQM
function# str&s ;;returns: 0 true, 1 false, 2 failed to resolve

 Resolves #if[[n]def] expression.

int iftype ;;0 #if, 1 #ifdef, 2 #ifndef
sel s 2
	case "#ifdef*" iftype=1; s.get(s 6)
	case "#ifndef*" iftype=2; s.get(s 7)
	case "#if*" s.get(s 3)
	case "#elif*" s.get(s 5)
s.ltrim

if(iftype)
	if(!m_mc.Get(s) and !m_mcf.Get(s))
		 out s
		ret iftype=1
	 out s
	ret iftype=2

s.replacerx("\bdefined +(\w+)" "defined($1)")
int i=m_expr.EvalC(s)
err
	 out s
	ret 2
 out "%i  %s" i s

 ret 2
ret !i

