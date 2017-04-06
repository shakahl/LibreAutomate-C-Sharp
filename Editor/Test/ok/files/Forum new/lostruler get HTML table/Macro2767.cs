out
HtmlDoc d
d.SetOptions(2)
 g1
d.InitFromWeb("https://datatables.net/examples/data_sources/js_array.html")

rep 10 ;;wait for the javascript to finish
	1
	str s=d.GetHtml("table" "example")
	if(s.len) break
	out "waiting"
if(!s.len)
	out "RETRY"
	goto g1

out s
