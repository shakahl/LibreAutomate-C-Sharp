 MSHTML.IHTMLElement el=htm("B" "VirtualLab Data Recovery" "" win("Internet Explorer") 0 3 0x21)

 HtmlClick("A" "Internet" "" win("Internet Explorer") 0 36 0x21)
 MSHTML.IHTMLElement el=htm("B" "Download Now" "" win("Internet Explorer") 0 4 0x21)
 el.click

 HtmlSetFocus("INPUT" "qt" "" win("Internet Explorer") 0 0 0x221)

 MSHTML.IHTMLElement el=htm("B" "OUR FAVORITE FREEWARE" "" win("Internet Explorer") 0 52 0x21)
 el.scrollIntoView
 MSHTML.IHTMLElement el=htm("BODY" "" "" win("Internet Explorer") 0 0 0x20)
 MSHTML.IHTMLElement2 el2=+el; el2.doScroll("pageDown")

 MSHTML.IHTMLElement el=htm("SELECT" "tg" "" win("Internet Explorer") 0 0 0x221)
 RECT r; HtmlLocation(el &r)
 out "%i %i %i %i", r.left r.top r.right r.bottom

 MSHTML.IHTMLElement el=htm("B" "VirtualLab Data Recovery" "" win("Internet Explorer") 0 3 0x21)
 out el.innerText
 out el.outerHTML
 out el.tagName

 MSHTML.IHTMLElement el=htm("A" "NetCaptor 7.5.1" "" win("Internet Explorer") 0 25 0x21)
 str attr=el.getAttribute("href" 0)
 out attr

 MSHTML.IHTMLElement el=htm("SELECT" "tg" "" win("Internet Explorer") 0 0 0x221)
 MSHTML.IHTMLSelectElement elsel=+el; elsel.selectedIndex=2
 MSHTML.IHTMLSelectElement elsel=+el; int i=elsel.selectedIndex
 out i

 MSHTML.IHTMLElement el=htm("INPUT" "5" "" win("Internet Explorer") 0 6 0x421)
 MSHTML.IHTMLInputElement elinp=+el
 if(elinp.checked)
	 out 1

 MSHTML.IHTMLElement el=htm("A" "IQ 4.0" "" win("Internet Explorer") 0 117 0x1)
 if(!el) ret
 out 1

 MSHTML.IHTMLElement el=htm("B" "ArtRage" "" win("Internet Explorer") 0 54 0x21)
 MSHTML.IHTMLDocument3 doc3=el.document; str dochtml=doc3.documentElement.outerHTML
 ShowText("" dochtml)

 MSHTML.IHTMLElement el=htm("B" "EditPlus" "" win("Internet Explorer") 0 56 0x21)
 MSHTML.IHTMLDocument2 doc=el.document; str url=doc.url
 out url

 MSHTML.IHTMLElement el=htm("DIV" "[]Here are five free downloads we think you'll like. [][]1. ArtRage[]Paint your own images using oils, pens, and other tools.[][]2. EditPlus[]Edit pla" "" win("Internet Explorer") 0 12 0x20)
 MSHTML.IHTMLDocument2 doc=el.document; doc.parentWindow.navigate("url2")

 HtmlFromToAcc(el a 0)
