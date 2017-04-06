ARRAY(POSTFIELD) p

out
HtmlDoc d.InitFromFile("$desktop$\google.htm")
out d.GetHtml("FORM" "f")
out "------------------------------------------"
ARRAY(MSHTML.IHTMLElement) a; int i
  d.GetHtmlElements(a "FORM")
 d.GetHtmlElements(a "INPUT" "FORM" "f")
 
 for i 0 a.len
	 out a[i].innerText
	 out a[i].outerHTML
	 out "-------"
d.GetForm("f" p)
out "------------------------------------------"
for i 0 p.len
	out p[i].name
	out p[i].value
	out p[i].isfile
	out "---"
	
