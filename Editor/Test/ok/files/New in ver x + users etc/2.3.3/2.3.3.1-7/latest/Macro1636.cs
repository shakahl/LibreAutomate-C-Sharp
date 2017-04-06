out
str s="one two, three"
int i
for i 0 5
	 out findt(s i)
	 out findt(s i ",")
	 out findt(s i "," 0x200)
	 out findt(s i 0)
	 out findt(s i 1)
	 out findt(s i 2)
	
	 out findt(s -i)
	 out findt(s)
	
	 out _s.gett(s i)
	 out _s.gett(s -i)
	 out _s.gett(s)
	 out _s.gett(s -i "" 2)
	
	 out _s.gett(s -i 2 0)
	
	 out _s
