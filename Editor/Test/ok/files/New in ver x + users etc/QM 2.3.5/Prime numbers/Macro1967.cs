out
int i p1 p2
Q &q
for i 0 100
	p1=IsPrimeNumber(i)
	p2=IsPrimeNumber2(i)
	 p2=IsPrimeNumberC(i)
	 p2=IsPrimeNumber_simplest(i)
	if(p1 or p2) out "%i:  %i %i" i p1 p2
	 if(p1!=p2) out "%i:  %i %i" i p1 p2
Q &qq
outq
