 /
function str&tsv ICsv&csv

 Loads string in TSV (tab-separated-values) format into ICsv variable.

 tsv - variable containing data in TSV format. <link>http://www.cs.tut.fi/~jkorpela/TSV.html</link>
 csv - variable of ICsv type. The function creates its data. Separator is comma.

 REMARKS
 TSV format is simpler than CSV, and has several limitations. Often data in TSV format cannot be loaded into an ICsv variable with FromString or FromFile, usually because it may contain double quote characters. Use this function instead.

 EXAMPLE
 str tsv="A[9]B[]C[9]D" ;;[9] is tab, [] is newline
 ICsv csv
 ConvertTsvToCsv tsv csv
 csv.ToString(_s); out _s


csv=CreateCsv(1)
str s
foreach s tsv
	int n=s.findreplace("[9]" "" 16)
	csv.AddRowMS(-1 n+1 s)

err+ end _error
