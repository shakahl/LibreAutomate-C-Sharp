function'MSHTML.IHTMLElement $tag [`nameOrIndex]

 Returns specified html element object as COM interface.
 Returns 0 if not found.

 tag - element's tag name. Examples: "TABLE", "A", "title". Can be "" if nameOrIndex used.
 nameOrIndex - element's id or name attribute, or 0-based index in collection of elements of tag. Default: 0.
   Can be omitted or 0 if there is only one element with this tag.
   QM 2.4.4: can be MSHTML.IHTMLElement or derived COM interface; then ignores tag and returns nameOrIndex as MSHTML.IHTMLElement.

 EXAMPLE
 HtmlDoc d.InitFromFile("$desktop$\html.txt")
 MSHTML.IHTMLElement el=d.GetHtmlElement("A" 10)
 out el.innerText


sel nameOrIndex.vt
	case VT_DISPATCH ret nameOrIndex.pdispVal
	case VT_BSTR if(!nameOrIndex.bstrVal.len) nameOrIndex=0 ;;"" -> 0

if(empty(tag)) ret d.all.item(nameOrIndex)
ret d3.getElementsByTagName(tag).item(nameOrIndex)
err+
