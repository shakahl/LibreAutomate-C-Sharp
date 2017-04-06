 /
function'MSHTML.IHTMLElement $text [flags] [$tag] [$containerTag] [`containerNameOrIndex] [int&index] ;;flags: 0 exact, 1 contains, 2 wildcard, 3 regex, 4 in HTML

 Finds HTML element containing certain text.
 Error if something fails, eg container not found.
 Returns 0 if element not found.

 text - element's text or HTML (flag 4). It can be exact text, or part (flag 1), or wildcard expression (flag 2) or regular expression (flag 3). Must match case (regex has option (?i) for case insensitive).
 tag - if used, searches only elements of this type. Example: "td".
 containerTag, containerNameOrIndex - if used, searches only elements within this element. It for example can be a table. See <help>HtmlDoc.GetHtmlElement</help>. QM 2.4.4: containerNameOrIndex also can be container's MSHTML.IHTMLElement or derived COM interface; then containerTag is ignored.
 index - if used, receives element index. If tag used, in collection of tag elements, else in all elements.


ARRAY(MSHTML.IHTMLElement) a
this.GetHtmlElements(a tag containerTag containerNameOrIndex)
int i; str s
for i 0 a.len
	MSHTML.IHTMLElement& el=a[i]
	if(flags&4) s=el.outerHTML; else s=el.innerText
	sel flags&3
		case 0 if(s=text) break
		case 1 if(find(s text)>=0) break
		case 2 if(matchw(s text)) break
		case 3 if(findrx(s text)>=0) break
if(i=a.len) ret
if(&index) index=i
ret el

err+ end _error
