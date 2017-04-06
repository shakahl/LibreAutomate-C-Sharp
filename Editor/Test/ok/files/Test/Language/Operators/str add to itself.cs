str s="a"
int i
for i 0 10
	 s+s
	 s+s.lpstr
	 s-s
	 s-s.lpstr
	 s.from(s s)
	 s.from(s s.lpstr)
	 s.from(s.lpstr s)
	 s.from(s.lpstr s.lpstr)
	out s
