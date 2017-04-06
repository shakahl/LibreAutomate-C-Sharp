out
HtmlDoc d
d.SetOptions(2)
d.InitFromWeb("http://www.quickmacros.com")
 out d.GetHtml

ARRAY(str) a; d.GetTable(1 a) ;;press F1 on GetTable for more examples
 display text in first cell of each row
int i ncolumns=2
for i 0 a.len ncolumns
	str f.format("%-20s %-20s" a[i] a[i+1])
	out f
