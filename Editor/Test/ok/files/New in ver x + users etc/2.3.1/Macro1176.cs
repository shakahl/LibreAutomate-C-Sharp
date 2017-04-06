out
str s=" one two, three"
str ss
int i k
for i 0 100
	 k=findt(s i)
	 k=findt(s i ",")
	 k=findt(s -i 0)
	 out k
	
	 k=ss.gett(s i "")
	k=ss.gett(s -i "")
	out "%i '%s'" k ss
	
	if(k<0) break
	