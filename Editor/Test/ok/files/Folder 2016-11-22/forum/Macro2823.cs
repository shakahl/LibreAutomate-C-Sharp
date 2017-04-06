out
HtmlDoc d.InitFromWeb("http://www.w3schools.com/html/html_tables.asp")
ARRAY(str) a
int t
for t 0 1000
	d.GetTable(t a)
	out F"<><Z 0xa080>table {t}</Z>"
	int i
	for i 0 a.len 2
		out "%s=%s" a[i] a[i+1]
		
