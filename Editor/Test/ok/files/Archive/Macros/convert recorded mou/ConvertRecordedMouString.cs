 /
function $s str&so [$prefix]

 Converts recorded mou command, where it is in format mou "...", to multiple mou- x y commands.

 s - input string.
 so - output string.
 prefix - a string to prepend to each line.


if(empty(s)) end ES_BADARG
sel(s[0]) case ['.',':'] s+1

int i j x y isy c w; str ss

for j 0 1000000
	c=s[0]
	 out s
	sel c
		case 0 break
		
		case ['+','-']
		for isy 0 2
			s+1
			for(i 0 100) if(!isdigit(s[i])) break
			if(i=0) end ES_BADARG
			y=val(ss.left(s i)); if(s[-1]=='-') y=-y
			if(!isy) x=y; if(s[i]!='+' && s[i]!='-') end ES_BADARG
			s+i
		s-1
		so.formata("%smou- %i %i[]" prefix x y)
		
		case ['w','f'] ;;wait, factor
		if(!isdigit(s[1])) goto g1
		s+1
		for(i 1 100) if(!isdigit(s[i])) break
		w=val(ss.left(s i))
		if(w) so.formata("%s%g%s[]" prefix w*0.001 iif(s[-1]='f' "*F" ""))
		s+i-1
		
		case else
		 g1
		if(c!='@' and !isalpha(c)) end ES_BADARG
		x=iif(c<=90 (c-64) -(c-96)); s+1; c=s[0]
		y=iif(c<=90 (c-64) -(c-96))
		so.formata("%smou- %i %i[]" prefix x y)
	
	s+1
