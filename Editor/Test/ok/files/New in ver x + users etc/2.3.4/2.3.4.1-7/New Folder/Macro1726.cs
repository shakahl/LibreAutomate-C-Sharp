out

 str tsv="A[9]B[]C[9]D"
 str tsv="[9]B[]C[9]"
 str tsv="A[9]B[]C[9]D[9]E"
 str tsv="A''[9]B[]C[9]D"
str tsv="A[9][9]B[]C[9][9]D"

ICsv csv
ConvertTsvToCsv tsv csv

csv.ToString(_s); out _s
 