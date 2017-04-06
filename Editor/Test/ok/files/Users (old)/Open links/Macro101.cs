act "Maxthon"

str s
MSHTML.IHTMLDocument3 doc=htm(win)
MSHTML.IHTMLElement el
foreach el doc.getElementsByTagName("A")
	s=el.getAttribute("href" 0)
	if(!s.begi("http")) continue ;;javascript, anchor, etc
	out s
	
act
