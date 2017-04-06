 /CtoQM
str k v s rx untag; int A
m_mut.EnumBegin
rep
	if(!m_mut.EnumNext(k v)) break
	A=0
	 g1
	s=m_mall.Get(k)
	if(!s.len)
		 out k
		continue
	 if(k=v) out k; continue
	 out "%s, in %s" v k
	
	if(m_mi.Get(v))
		 out "%s, in %s" v k
		s.replacerx(rx.format("(?<=\b%s)\*(?=\w)" v) "'")
		s.replacerx(rx.format("(?<=\b%s)\*\*" v) "*")
	else if(k=v) continue
	else if(m_mtag.Get2(v untag))
		if(untag.end("A"))
			untag.fix(untag.len-1)
			if(!m_mtd.Get(untag)) untag+"A"
		 out "%s=%s, in %s" v untag k
		s.findreplace(v untag 2)
		 out s
	else if(m_mt.Get2(v untag))
		 out "%s, in %s" v k
	else
		 out "%s, in %s" v k
		s.replacerx(rx.format("\b(%s)\*" v) "!*")
		 out s
		 continue
	 out "%s, in %s" v k
	m_mall.Set(k s)
	if(!A and k.end("A")) A=1; k.fix(k.len-1); if(k.len) goto g1
	
		
		
