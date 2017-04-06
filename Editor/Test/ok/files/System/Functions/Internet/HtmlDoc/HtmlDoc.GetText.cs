function~ [$containerTag] [`containerNameOrIndex]

 Gets text without HTML tags.
 Error if something fails, eg container not found.

 containerTag, containerNameOrIndex - if used, gets text only of this element. Else gets body text. See <help>HtmlDoc.GetHtmlElement</help>. QM 2.4.4: containerNameOrIndex also can be container's MSHTML.IHTMLElement or derived COM interface; then containerTag is ignored.

 EXAMPLES
 HtmlDoc d.InitFromWeb("http://www.quickmacros.com/index.html")
 str s=d.GetText
 out s
 out d.GetText("title")
 out d.GetText("table" 3)


MSHTML.IHTMLElement el
if(IsCont(&containerTag))
	sel containerTag 1
		case "body" el=d.body
		case "title" ret d.title
		case else el=GetHtmlElement(containerTag containerNameOrIndex)
else el=d.body

if(!el) end F"{ERR_OBJECT} (container)"

ret el.innerText

err+ end _error
