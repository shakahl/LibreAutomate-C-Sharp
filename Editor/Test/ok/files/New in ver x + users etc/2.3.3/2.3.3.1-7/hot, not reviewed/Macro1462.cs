out
HtmlDoc d.InitFromWeb("http://www.w3schools.com/html/html_tables.asp")
ARRAY(str) a
for _i 0 100
	d.GetTable(_i a) ;;get the second table
	out _i
	out a[0]
