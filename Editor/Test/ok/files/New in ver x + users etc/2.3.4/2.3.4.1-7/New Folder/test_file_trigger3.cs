rep
	str r1.RandomString(5 30 "a-z") r2.RandomString(5 30 "a-z")
	str f1=F"$temp$\qm____{r1}"
	str f2=F"{f1}\{r2}.txt"
	mkdir f1
	str s.RandomString(10 1000000)
	s.setfile(f2)
	0.5
	del- f2
	del- f1
	