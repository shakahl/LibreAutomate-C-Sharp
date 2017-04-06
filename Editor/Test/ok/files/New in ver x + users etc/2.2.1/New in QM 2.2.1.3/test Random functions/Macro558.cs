out
int i i0 i1 i2
 int i=RandomNumber(0 2)
 out i
rep 20
	 out RandomInt(-2 2)
	out RandomDouble(-2 2)
		
	 i=RandomInt(0 2)
	 out i
	 sel i
		 case 0 i0+1
		 case 1 i1+1
		 case 2 i2+1
		 case else mes i
		 
	 i=RandomInt(-2 0)
	 out i
	 sel i
		 case 0 i0+1
		 case -1 i1+1
		 case -2 i2+1
		 case else mes i
		
out "%i %i %i" i0 i1 i2
