act "Internet Explorer"

 with download.com

 click
 MSHTML.IHTMLElement el=htm("A" "Internet" "" win("Internet Explorer") 0 23 0x21)
 el.click

 select
 MSHTML.IHTMLElement el=htm("INPUT" "qt" "" win("Internet Explorer") 0 0 0x221)
 MSHTML.IHTMLElement2 el2=+el; el2.focus

 scroll
 MSHTML.IHTMLElement el=htm("B" " MOST POPULAR" "" "Internet Explorer" 0 71 0x21)
 el.scrollIntoView

 scroll2
 MSHTML.IHTMLElement el=htm("BODY" "" "" win("Internet Explorer") 0 0 0x20)
 MSHTML.IHTMLElement2 el2=+el; el2.doScroll("pageDown")

 location
 MSHTML.IHTMLElement el=htm("A" "Games" "" win("Internet Explorer") 0 40 0x21)
 RECT r; HtmlLocation(el &r)
 out "%i %i" r.left r.top

 text, html, tag, attribute
 MSHTML.IHTMLElement el=htm("A" "Utilities & Drivers" "" win("Internet Explorer") 0 62 0x21)
 str s=el.innerText
 out s
 s=el.outerHTML
 out s
 s=el.tagName
 out s
 s=el.getAttribute("href" 0)
 out s
 s=el.getAttribute("href" 2)
 out s

 select, get selected
 MSHTML.IHTMLElement el=htm("SELECT" "tg" "" win("Internet Explorer") 0 0 0x221)
 MSHTML.IHTMLSelectElement elsel=+el; elsel.selectedIndex=1
 elsel=+el; int i=elsel.selectedIndex
 out i

 if checked
 MSHTML.IHTMLElement el=htm("INPUT" "2" "" win("Internet Explorer") 0 3 0x421)
 MSHTML.IHTMLInputElement elinp=+el
 if(elinp.checked)
	 out "yes"

 if not found
 MSHTML.IHTMLElement elo=htm("A" "Utilities & Drivers" "" win("Internet Explorer") 0 62 0x1)
 if(!elo) ret
 out "found"

 get doc html, url
 MSHTML.IHTMLElement el=htm("BODY" "" "" win("Internet Explorer") 0 0 0x20)
 MSHTML.IHTMLDocument3 doc3=el.document; str dochtml=doc3.documentElement.outerHTML
 ShowText "" dochtml
 MSHTML.IHTMLDocument2 doc=el.document; str url=doc.url
 out url

 navigate
 MSHTML.IHTMLElement el=htm("BODY" "" "" win("Internet Explorer") 0 0 0x20)
 MSHTML.IHTMLDocument2 doc=el.document; doc.parentWindow.navigate("http://www.quickmacros.com")

 get acc, from acc
 MSHTML.IHTMLElement el=htm("A" "Utilities & Drivers" "" win("Internet Explorer") 0 62 0x21)
 
 Acc a
 HtmlFromToAcc(el a 1)
 out a.Name
 el=0
 HtmlFromToAcc(el a 0)
 out el.outerHTML
