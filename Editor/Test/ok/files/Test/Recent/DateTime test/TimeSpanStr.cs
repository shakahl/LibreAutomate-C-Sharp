out
DateTime x

str s=
 1
 1 2
 1 2:3
 1 2:3:4
 1 2:3:4.5
 1 2:3:4.005
 2:3
 2:3:4
 2:3:4.5
 2:3:4.005
 3:4.5
 3:4.005
 4.5
 4.005
 4.0000005
 -1 2:3:4
 -2:3:4.5

str ss
foreach ss s
	long k=TimeSpanFromStr(ss)
	out TimeSpanToStr(k 4)
