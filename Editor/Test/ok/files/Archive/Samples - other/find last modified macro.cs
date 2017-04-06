out

DATE d; int iid
QMITEM q; int i
rep
	i=qmitem(-i 1|2 &q 128)
	if(i=0) break
	
	if(q.datemod>d)
		d=q.datemod
		iid=i

out _s.getmacro(iid 1)
