out

int i j nt fl ntt
for i 0 12
	str s=".one, two, three"
	
	 ARRAY(str) a aa
	ARRAY(lpstr) a aa
	 a=0; aa=0
	nt=iif(i<4 -1 iif(i<8 2 4))
	 nt=iif(i<4 -1 iif(i<8 2 3))
	
	 str s1 s2 s3 s4; str* a=&s1; str se1 se2 se3 se4; str* aa=&se1
	 lpstr s1 s2 s3 s4; lpstr* a=&s1; lpstr se1 se2 se3 se4; lpstr* aa=&se1
	 nt=iif(i<4 2 4); if(i=8) break
	 aa=0
	
	fl=i&3
	ntt=tok(s a nt "" fl aa)
	 out "%i %i %i" ntt a.len aa.len
	out ntt

	out "---- nt=%i, flags=%i ----" nt fl
	if(aa) for(j 0 ntt) out "%s|%s(%i)" a[j] aa[j] aa[j]
	else for(j 0 ntt) out "%s|" a[j]
	outb s s.len 1
