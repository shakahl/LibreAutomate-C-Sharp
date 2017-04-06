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
str s

ARRAY(str) ab; int ib
if(!findrx(logfiledata "(?s).+?[](?=[][][]|$)" 0 4 ab)) ret ;;split blocks; return if empty
for ib 0 ab.len
	out ab[0 ib]; out "---"
	 foreach s ab[0 ib]
		 i+1; if(i<3) continue ;;skip first two lines
		 if(!s.len)
			 if(!j) ;;end of block
				 j=1
				 ib+1
				 out "block %i" ib
			 continue
		 
		 if(!s.begi("urgent")) continue
		 X765& e=a[a.redim(-1)] ;;add 1 new element and get reference to it
		 if(tok(s &e.u 6 " ")!=6) end "bad format"
		 out "%s, %s, %s, %s, %s, %s" e.u e.a e.b e.c e.d e.e



	
