out
QMITEM q; int i j k; str s
i=qmitem("\System")
 out i
rep
	i=qmitem(-i 1 &q 1|8)
	if(i=0) break
	 out "%s" q.name
	
	ARRAY(CHARRANGE) a
	if findrx(q.text "<help>(.+?)</help>" 0 4 a)
	 if findrx(q.text "<tip>(.+?)</tip>" 0 4 a)
		for j 0 a.len
			s.get(q.text a[0 j].cpMin (a[0 j].cpMax-a[0 j].cpMin))
			out "<>%-50s  in <open ''%s /%i''>%s</open>" s q.name (a[0 j].cpMin+6) q.name
	
	ARRAY(str) as
	k=findrx(q.text "See also: *(<.+>)" 0 0 s 1)
	if k>=0
		if findrx(s "<(.+?)>" 0 4 as)
			for j 0 as.len
				out "<><help>%s</help>               in <open ''%s /%i''>%s</open>" as[1 j] q.name k q.name
