 /CtoQM

 Appends missing GUIDs to interfaces.

str s1 s2 s3
lpstr s4

m_mi.EnumBegin
rep
	if(!m_mi.EnumNext(s1 s2)) break
	if(!s2.end("}"))
		s3.from("IID_" s1)
		s4=m_mg.Get(s3); if(!s4) s3+"A"; s4=m_mg.Get(s3)
		 out "%s %s" s1 s4
		if(s4 and findrx(s4 "\{.+\}" 0 0 s3)>=0)
			s2.formata("[][9]%s" s3)
			m_mi.Set(s1 s2)
		 else out s1


