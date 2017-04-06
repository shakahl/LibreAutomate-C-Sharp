out
HtmlDoc d.InitFromWeb("http://www.quickmacros.com/help/Tables/IDP_ASCII.html")
ARRAY(str) a
d.GetTable(1 a) ;;get the second table
int i
for i 0 a.len 6 ;;6 columns
	out a[i+4] ;;5-th column
