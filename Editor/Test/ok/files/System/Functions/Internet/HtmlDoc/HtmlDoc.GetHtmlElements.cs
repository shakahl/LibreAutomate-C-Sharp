function ARRAY(MSHTML.IHTMLElement)&a [$tag] [$containerTag] [`containerNameOrIndex]

 Gets all HTML elements or all elements of specified type (tag).
 Error if something fails, eg container not found.

 a - array variable for results.
 tag - if used, gets only elements of this type. Else gets all elements.
 containerTag, containerNameOrIndex - if used, gets only elements within this element. It for example can be a table. See <help>HtmlDoc.GetHtmlElement</help>. QM 2.4.4: containerNameOrIndex also can be container's MSHTML.IHTMLElement or derived COM interface; then containerTag is ignored.

 EXAMPLES
 HtmlDoc d.InitFromWeb("http://www.quickmacros.com/index.html")
 ARRAY(MSHTML.IHTMLElement) a; int i
 d.GetHtmlElements(a "A")
 for i 0 a.len
	 out a[i].innerText
	 out a[i].outerHTML
	 out a[i].getAttribute("href" 2)
	 out "-------"

 d.GetHtmlElements(a "A" "TABLE" 5)
 for i 0 a.len
	 out a[i].innerText
	 out a[i].outerHTML


MSHTML.IHTMLElementCollection c
if(IsCont(&containerTag))
	MSHTML.IHTMLElement ec=GetHtmlElement(containerTag containerNameOrIndex)
	if(!ec) end F"{ERR_OBJECT} (container)"
	if(!empty(tag))
		MSHTML.IHTMLElement2 ec2=+ec
		c=ec2.getElementsByTagName(tag)
	else c=ec.all
else
	if(!empty(tag)) c=d3.getElementsByTagName(tag)
	else c=d.all

a.redim(c.length)
for(_i 0 a.len) a[_i]=c.item(_i)

err+ end _error
