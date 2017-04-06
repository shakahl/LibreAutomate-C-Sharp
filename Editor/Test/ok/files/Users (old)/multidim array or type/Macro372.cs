 str logfiledata.getfile(...)
str logfiledata=
 header
 
 not urgent a b c d e
 urgent a b c d e
 urgent a b c d e
 not urgent a b c d e
 urgent a b c d e
 
 
 
 not urgent a b c d e
 urgent a b c d e
 urgent a b c d e
 not urgent a b c d e
 urgent a b c d e
 

type X765 str'u str'a str'b str'c str'd str'e ;;declare type to hold 6 words
ARRAY(X765) a
str s; int i ie ib
logfiledata+"[][][][]" ;;add several empty lines

foreach s logfiledata
	i+1; if(i<3) continue ;;skip first two lines
	if(!s.len) ;;empty line
		ie+1 ;;count empty lines
		if(ie=3) ;;end of block
			ib+1
			out "end of block %i" ib
		continue
	
	ie=0
	if(!s.begi("urgent")) continue
	X765& e=a[a.redim(-1)] ;;add 1 new element and get reference to it
	if(tok(s &e.u 6 " ")!=6) end "bad format"
	out "%s, %s, %s, %s, %s, %s" e.u e.a e.b e.c e.d e.e
	
