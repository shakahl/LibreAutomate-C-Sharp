 /
function str&tsv ARRAY(str)&a

 Creates string in TSV (tab-separated-values) format from array variable.

 tsv - variable that receives TSV.
 a - 2-dim array variable.

 EXAMPLE
 str tsv="A[9]B[]C[9]D" ;;[9] is tab, [] is newline
 ARRAY(str) a
 TsvToArray tsv a
 str tsv2
 TsvFromArray tsv2 a
 out tsv2


tsv.all
int r c nc(a.len(1))
for r 0 a.len
	for c 0 nc
		tsv+a[c r]
		tsv+iif(c=nc-1 "[]" "[9]")

err+ end _error
