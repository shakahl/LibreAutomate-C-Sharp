 /CtoQM

 Removes FuncW if Func exists.

str k s
m_mf.EnumBegin
rep
	if(!m_mf.EnumNext(k 0)) break
	if(k[k.len-1]!='W') continue
	s.left(k k.len-1)
	if(m_mf.Get(s)) m_mf.Remove(k);; out k

 m_mt.EnumBegin
 rep
	 if(!m_mt.EnumNext(k 0)) break
	 if(k[k.len-1]!='W') continue
	 s.left(k k.len-1)
	 if(m_mt.Get(s)) out k

 m_mc.EnumBegin
 rep
	 if(!m_mc.EnumNext(k 0)) break
	 if(k[k.len-1]!='W') continue
	 s.left(k k.len-1)
	 if(m_mc.Get(s)) out k
