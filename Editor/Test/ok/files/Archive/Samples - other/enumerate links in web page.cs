Htm el=htm("BODY" "" "" "+IEFrame" 0 0 0x20) ;;get BODY element which is container for all visible elements
MSHTML.IHTMLElement2 el2=+el.el ;;get IHTMLElement2 interface because we need getElementsByTagName function
MSHTML.IHTMLElement el3
foreach el3 el2.getElementsByTagName("A") ;;enumerate hyperlinks and anchors
	str linktext=el3.innerText ;;get link text
	if(!linktext.len) continue
	out linktext ;;remove this
	if(linktext.beg("DeflautLink"))
		el3.click
