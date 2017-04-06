 /
function str&tsv ARRAY(str)&a

 Loads string in TSV (tab-separated-values) format into array variable.

 tsv - variable containing data in TSV format. <link>http://www.cs.tut.fi/~jkorpela/TSV.html</link>
 a - variable. The function creates its data as 2-dim array.

 EXAMPLE
 str tsv="A[9]B[]C[9]D" ;;[9] is tab, [] is newline
 ARRAY(str) a
 TsvToArray tsv a
 int r c
 for r 0 a.len
	 out "row %i" r
	 for c 0 a.len(1)
		 out a[c r]


a=0
str sr sc
int r c nc
foreach sr tsv
	nc=sr.findreplace("[9]" "[10]")+1
	if(!a.len) a.create(nc 1)
	else if(nc<=a.len(1)) a.redim(-1)
	else end "incorrect format"
	c=0
	foreach sc sr
		a[c r]=sc
		c+1
	r+1

err+ end _error
