 /CtoQM
function $t $p str&q funcret $container [str&cb]

 Converts typename, eg DWORD to #.
 Used to convert function argument types, function return types, etc.

int ptr ln csym ist; str s st untag

if(p) rep() if(p[0]='*') p+1; ptr+1; else break
if(Unalias(t st ptr 0 cb)) t=st
if(t[0]='#' and !funcret) t+1
q=t

if q.len and __iscsym(q[0])
	csym=1;; if(!__iscsym(q[q.len-1])) out q

	if(m_mtag.Get2(q untag))
		 out "%s %s" q untag
		q=untag
	
	if(m_mt.Get(q))
		if(q.end("A"))
			s.left(q q.len-1); if(s.end("_")) s.fix(s.len-1)
			p=m_mtd.Get(s); if(!p) p=m_mc.Get(s)
			if(q=p) q=s ;;out "%-30s  %s" q container
			 else if(!q.end("DATA") and !q.end("PARA")) out "%-30s  %-30s %s" q p container
	else if(ptr)
		if(m_mi.Get(q)) ptr-1
		else if(m_mtd.Get(q)) ;;out q
		else AddToMap(m_mut container q "" 1);; out "%s, in %s" q container;;forward-declared or itself

if(ptr) rep(ptr) q+"*"
else if(csym) q+"'"

 if(csym) out q
 if(q.beg("H")) out q ;;HANDLE__ must be set to # in AddTypes
