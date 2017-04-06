str s1.all(256 2 0) s2.all(256 2 0)

int t1(GetCurrentThreadId) t2 notfirst

rep
	Q &q
	int i j
	for(i 8 VK_F13)
		 j=GetKeyState(i)
		j=GetTickCount
		if(j&0x8000) j|0x80
		s1[i]=j
	Q &qq
	outq
	 outb s1 256
	if(memcmp(s1 s2 256))
		memcpy s2 s1 256
		if(!notfirst) notfirst=1; continue
		out "changed"
	1

#ret
