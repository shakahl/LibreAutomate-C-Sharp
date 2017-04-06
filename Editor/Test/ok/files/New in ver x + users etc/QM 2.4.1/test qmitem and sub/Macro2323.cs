out
QMITEM q; int i l m
rep
	i=qmitem(-i 0 &q 1|16)
	if(i=0) break
	 out i
	out "%i %s" i q.name
	m+1; if(m=13000) break
	 out m
	 if q.itype!5 or q.folderid; continue
	 l+1
	 out "%i %i %i - %s" l i q.folderid q.name
