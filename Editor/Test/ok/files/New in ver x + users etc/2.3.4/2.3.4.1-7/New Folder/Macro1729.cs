out

str tsv="A[9]B[]C[9]D"
 str tsv="[9]B[]C[9]"
 str tsv="A[9]B[]C[9]D[9]E" ;;error
 str tsv="A[9]B[9]BB[]C[9]D"
 str tsv="A''[9]B[]C[9]D"
 str tsv="A[9][9]B[]C[9][9]D"
 str tsv="[9][9][][9][9]"

ARRAY(str) a
TsvToArray tsv a

 int r c
 for r 0 a.len
	 out "row %i" r
	 for c 0 a.len(1)
		 out a[c r]

str tsv2
TsvFromArray tsv2 a
out tsv2
