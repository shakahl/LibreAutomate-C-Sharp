 /CtoQM

 Converts some typedefs to type, interface, etc.

str s1 s2 ss sg

m_mtd.EnumBegin
rep
	if(!m_mtd.EnumNext(s1 s2)) break
	if(!__iscsym(s2[0])) continue
	if(!s2.end("*"))
		if(m_mt.Get(s1)) continue
		if(m_mt.Get2(s2 ss))
			 out "%s %s" s1 ss
			ss.findreplace(s2 s1 6)
			AddToMap(m_mt s1 ss)
			if(m_mut.Get2(s2 ss)) AddToMap(m_mut s1 ss "" 1);; out "%s, in %s" ss s1
		else
			if(m_mi.Get2(s2 ss)) ;;typedef Interface1 Interface2
				 out "%s %s[]%s" s1 s2 ss
				ss.findreplace(s2 s1 6)
				AddToMap(m_mi s1 ss)
			else
				ss.format("type %s :%s'_" s1 s2)
				AddToMap(m_mt s1 ss)
		 out "%s %s" s1 s2
	else
		s2.fix(s2.len-1)
		ss=s2; ss.rtrim("*")
		if(m_mi.Get(ss)) ;;INterface* to INterface
			 out "%s %s" s1 s2
			m_mtd.Set(s1 s2)
