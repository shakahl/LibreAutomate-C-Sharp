out
HtmlDoc d.InitFromFile("$desktop$\html2.txt")

ARRAY(MSHTML.IHTMLElement) a aa; int i
d.GetLinks(a)
d.GetHtmlElements(aa "A")

for i 0 a.len
	out a[i].outerHTML
	_s=a[i].getAttribute("href" 2)
	out _s

out "-------------------"

for i 0 aa.len
	out aa[i].outerHTML
	out aa[i].getAttribute("href" 2)

 out
 HtmlDoc d.InitFromFile("$desktop$\html2.txt")
 
 MSHTML.IHTMLElementCollection c cc
 MSHTML.IHTMLElement e
 c=d.d.links
 cc=d.d3.getElementsByTagName("A")
 
 foreach e c
	 out e.outerHTML
 
 out "-------------------"
 
 foreach e cc
	 out e.outerHTML
