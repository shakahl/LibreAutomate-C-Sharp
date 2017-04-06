 /CtoQM
str s k v; int i
IStringMap* a=&m_mf

for i 0 9
	s.formata(".%i[]" i)
	IStringMap& m=a[i]
	m.EnumBegin
	rep
		if(!m.EnumNext(k v)) break
		s.formata("%s %s[]" k v)

s.setfile(_s.from(m_dest "_pch.txt"))
