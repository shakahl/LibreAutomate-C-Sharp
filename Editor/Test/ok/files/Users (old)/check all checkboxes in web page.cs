Htm el=htm("BODY" "" "" "+IEFrame" 0 0 0x20)
MSHTML.IHTMLElement2 el2=+el.el
MSHTML.IHTMLElementCollection col=el2.getElementsByTagName("INPUT")
out col.length
MSHTML.IHTMLElement el3
foreach el3 col
	str st=el3.getAttribute("type" 0)
	if(st~"checkbox")
		el3.setAttribute("checked" "true" 1)
		 el3.removeAttribute("CHECKED" 2)
		 el3.click
		out "---"
	

