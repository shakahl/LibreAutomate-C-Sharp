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
