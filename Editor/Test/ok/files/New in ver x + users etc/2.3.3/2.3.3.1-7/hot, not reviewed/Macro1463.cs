out
HtmlDoc d.InitFromWeb("http://www.w3schools.com/html/html_tables.asp")
ARRAY(str) a
d.GetTable(8 a)

int i
for i 0 a.len 2
	out "%s=%s" a[i] a[i+1]
