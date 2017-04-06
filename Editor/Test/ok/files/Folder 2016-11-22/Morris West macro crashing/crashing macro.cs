ARRAY(str)+ stringstack
if(!stringstack.len) stringstack.create(50)


opt clip 1
str s
int t
int x=49

s.getsel

if(len(s)=0 or (len(s)=2 and (s[1]=10 and s[2]=0)));; test to see if the selection is blank excel cell
	outp stringstack[0]
else
		rep 49
			stringstack[x]=stringstack[x-1]
			x-1
	if((s[1]<>10 and s[2]<>0) and (s[1]<>9 and s[2]<>9));; test to see if first chars are a blank excel cell
		s.trim
	stringstack[0]=s
	s.setclip
	signal_square

