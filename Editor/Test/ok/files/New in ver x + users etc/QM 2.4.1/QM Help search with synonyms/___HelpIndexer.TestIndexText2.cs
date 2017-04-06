 /CHI_CreateIndex

m_test.GetList(_s "=")
out _s

 ARRAY(str) a; int i
 m_test.GetAll(a)
 for i 0 a.len
	 _s=a[i]
	 _s.stem
	 out "<>%s <c 0xff0000>%s</c>" a[i] _s
	

out m_test.Count
 7241
