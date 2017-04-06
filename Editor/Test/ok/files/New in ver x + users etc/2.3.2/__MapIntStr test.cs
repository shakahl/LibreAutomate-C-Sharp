__MapIntStr x

str s=
 1 one
 3 three
 10ten
 11
 -20 minus twenty

x.AddList(s)

int i
for i 0 x.a.len
	LPSTRINT r=x.a[i]
	out "%i '%s'" r.i r.s
