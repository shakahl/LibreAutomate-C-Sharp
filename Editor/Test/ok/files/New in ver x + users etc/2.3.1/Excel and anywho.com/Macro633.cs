out
str First("Bill") Last("Gates") City State Zip

str AnyWhoURL.format("http://whitepages.anywho.com/results.php?&qc=%s&qf=%s&qn=%s&qs=%s&qz=%s" City First Last State Zip)
HtmlDoc d.InitFromWeb(AnyWhoURL)

ARRAY(str) a
d.GetTable(12 a 0 1)
 
int j
for j 0 a.len
	out a[j]
	out "-------"
ret
