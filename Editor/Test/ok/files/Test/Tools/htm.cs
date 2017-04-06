MSHTML.IHTMLElement el=htm("TD" "MB_OK" "" "MSDN Library - January 2002 - MessageBox" 0 10 0x21)
MSHTML.IHTMLDocument2 doc=el.document; doc.parentWindow.navigate("uuu")
HtmlClick("TD" "MB_CANCELTRYCONTINUE" "" "MSDN Library - January 2002 - MessageBox" 0 6 0x21)
MSHTML.IHTMLElement el=htm("TABLE" "ValueMeaning[]MB_ABORTRETRYIGNOREThe message box contains three push buttons: Abort, Retry, and Ignore.[]MB_CANCELTRYCONTINUEWindows 2000/XP: The mess" "" "MSDN Library - January 2002 - MessageBox" 0 2 0x20)
el.click
MSHTML.IHTMLElement el=htm("B" "WM_HELP" "" "MSDN Library - January 2002 - MessageBox" 0 20 0x21)
MSHTML.IHTMLElement2 el2=+el; el2.focus
MSHTML.IHTMLElement2 el2=+el; el2.focus
HtmlSetFocus("TABLE" "ValueMeaning[]MB_ABORTRETRYIGNOREThe message box contains three push buttons: Abort, Retry, and Ignore.[]MB_CANCELTRYCONTINUEWindows 2000/XP: The mess" "" "MSDN Library - January 2002 - MessageBox" 0 2 0x20)
MSHTML.IHTMLElement el=htm("TD" "MB_HELP" "" "MSDN Library - January 2002 - MessageBox" 0 8 0x21)
el.scrollIntoView
MSHTML.IHTMLElement el=htm("TD" "MB_HELP" "" "MSDN Library - January 2002 - MessageBox" 0 8 0x21)
RECT v99; HtmlLocation(el &v99)
MSHTML.IHTMLElement el=htm("TD" "MB_CANCELTRYCONTINUE" "" "MSDN Library - January 2002 - MessageBox" 0 6 0x21)
str text=el.innerText
HtmlSetText("jj" "TD" "MB_OKCANCEL" "" "MSDN Library - January 2002 - MessageBox" 0 12 0x21)
el.innerText=vv
str html=el.outerHTML
str tag=el.tagName
str attr=el.getAttribute("href" 0)
MSHTML.IHTMLSelectElement elsel=+el; elsel.selectedIndex=9
MSHTML.IHTMLSelectElement elsel=+el; int i=elsel.selectedIndex
MSHTML.IHTMLInputElement elinp=+el
if(elinp.checked)
	if(!el) ret
	MSHTML.IHTMLDocument3 doc3=el.document; str dochtml=doc3.documentElement.outerHTML
	MSHTML.IHTMLDocument2 doc=el.document; str url=doc.url
	