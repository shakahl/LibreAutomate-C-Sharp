function [str'First] [str'Last] [str'State] [str'City] [str'Zip]

str AnyWhoURL.format("http://whitepages.anywho.com/results.php?&qc=%s&qf=%s&qn=%s&qs=%s&qz=%s&qi=0&qk=100" City First Last State Zip)

out
HtmlDoc d.InitFromWeb(AnyWhoURL)

ARRAY(str) a Entry Information
d.GetTable(12 a 0 1)
int j
for j 0 a.len
	if(matchw(a[j] "<TABLE class=resultTable*"))
		goto Populate
	if(matchw(a[j] "*<SPAN class=singleName*"))
		goto Populate
		 Populate
		int flag=1
		d.InitFromText(a[j])
		d.GetTable(0 Entry)
		tok(Entry[0] Information 4 "[]" 16)
		out Information[0]
		out Information[1]
		out Information[2]
		out Information[3]
		out "-------"
if flag=1;ret
_s=Information;_i=val(_s);if _i=0;out "No Matches";ret

ret