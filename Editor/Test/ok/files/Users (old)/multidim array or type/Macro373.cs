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
 

ARRAY(str) a.create(6 0) ;;first dimension has 6 elements, one for each word; second dimension is resizable. Imagine that first dimension is horizontal (x), second vertical (y).
str s
foreach s logfiledata
	if(!s.len) continue
	if(!s.begi("urgent")) continue
	int ni=a.redim(-1) ;;add 1 new element (6-string array) in second dimension; ni is its index
	if(tok(s &a[0 ni] 6 " ")!=6) end "bad format"
	out "%s, %s, %s, %s, %s, %s" a[0 ni] a[1 ni] a[2 ni] a[3 ni] a[4 ni] a[5 ni]
	
