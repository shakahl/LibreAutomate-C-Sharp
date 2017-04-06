function~ [$containerTag] [`containerNameOrIndex] [flags] ;;flags: without container's HTML

 Returns HTML.
 Error if something fails, eg container not found.

 containerTag, containerNameOrIndex - if used, gets HTML only of this element. Else gets HTML of whole document, including header. See <help>HtmlDoc.GetHtmlElement</help>. QM 2.4.4: containerNameOrIndex also can be container's MSHTML.IHTMLElement or derived COM interface; then containerTag is ignored.

 REMARKS
 The HTML is not exactly the same as the page source because it is parsed/recreated.

 EXAMPLE
 HtmlDoc d.InitFromWeb("http://www.quickmacros.com/index.html")
 str s=d.GetHtml("body")
 out s


MSHTML.IHTMLElement el
if(IsCont(&containerTag))
	if(matchw(containerTag "body" 1)) el=d.body
	else el=GetHtmlElement(containerTag containerNameOrIndex)
else el=d3.documentElement

if(!el) end F"{ERR_OBJECT} (container)"

if(flags&1) ret el.innerHTML
ret el.outerHTML

err+ end _error
