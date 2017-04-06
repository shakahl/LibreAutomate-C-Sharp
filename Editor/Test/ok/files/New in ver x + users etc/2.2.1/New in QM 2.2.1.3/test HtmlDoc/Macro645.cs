out
HtmlDoc d.InitFromFile("$desktop$\html.txt")
 HtmlDoc d.InitFromWeb("http://www.quickmacros.com/index.html")
ARRAY(MSHTML.IHTMLElement) a; int i
 d.GetHtmlElements(a "A")
d.GetHtmlElements(a "A" "TABLE" 10)
for i 0 a.len
	out a[i].innerText
	out a[i].outerHTML
	 VARIANT v=a[i].getAttribute("href" 2)
	 out(v.vt)
	 out v
	out "-------"
