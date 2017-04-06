function $sin str&sout
int i lens(len(sin)) sp(1) qu
sout.all(lens)
for i 0 lens
	if(sin[i]=34) qu^1; sp=0
	else if(sin[i]=13 or sin[i]=10) sp=1; qu=0
	else if(qu=0)
		if sin[i]=32
			if(sp) continue
			sp=1
		else sp=0

	sout.set(sin[i] sout.len)

 replaces multiple spaces to single space
 for example, if sin is "one   two" then
 sout will be "one two"