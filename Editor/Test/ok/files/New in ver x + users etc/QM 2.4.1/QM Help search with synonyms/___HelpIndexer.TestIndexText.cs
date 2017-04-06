 /CHI_CreateIndex
function ~s

s.lcase
ARRAY(lpstr) a; int i
tok s a -1 "" 1
for i 0 a.len
	lpstr w=a[i]
	if(findcs(w "_0123456789")>=0) continue
	 out w
	 m_test.Add(w)
	
	str sStem=w; sStem.stem
	lpstr ww=m_test.Get(sStem)
	if ww
		if(findw(ww w)>=0) continue
		w=_s.from(ww " " w)
	m_test.Add(sStem w)
