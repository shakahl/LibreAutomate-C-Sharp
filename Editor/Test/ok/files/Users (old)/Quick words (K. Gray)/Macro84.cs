 Copy some text and run this macro ...

double wordspeed=0.15
double sentencespeed=0.5

str s.getclip
int t=GetTickCount
ARRAY(str) a
int i n=tok(s a -1 " [9][][160]")
for i 0 n
	ifk(C) break
	ClearOutput
	out a[i]
	if(a[i].end(".")) wait sentencespeed
	else wait (a[i].len*(wordspeed/10)+wordspeed)
ClearOutput
out "Time: %g" GetTickCount-t/1000.0
