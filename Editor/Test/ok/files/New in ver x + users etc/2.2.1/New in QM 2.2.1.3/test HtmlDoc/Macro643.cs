out
 HtmlDoc d.InitFromFile("$desktop$\html2.txt")
HtmlDoc d.InitFromWeb("http://www.quickmacros.com/index.html")
ARRAY(MSHTML.IHTMLElement) a; int i
d.GetLinks(a)
for i 0 a.len
	out a[i].innerText
	out a[i].outerHTML
	out a[i].getAttribute("href" 2)
	out a[i].getAttribute("target" 2)
	out "-------"
