function [flags] [ARRAY(str)&aURL] [ARRAY(str)&aText] [ARRAY(MSHTML.IHTMLElement)&aElem] ;;flags: 0 inner links, 1 all links, 2 itself is link

 Gets links.

 flags:
   0 get links that are inside this element (except from inner frames/iframes).
   1 get all links in element's container document (which may be frame/iframe).
   2 get properties of this element (it should be a link).
 aURL - receives href attribute of links. It is full URL, not relative as in HTML source. Can be 0 if you dont need it.
 aText - receives text of links. Can be 0 if you dont need it.
 aElem - receives COM interface of links. You can call its functions to get URL, text and other attributes. Can be 0 if you dont need it.

 EXAMPLE
 int w=win("Internet Explorer" "IEFrame")
 Htm e=htm("BODY" "" "" w "" 0 0x20)
 ARRAY(str) a at; int i
 e.GetLinks_(1 a at)
 for i 0 a.len
	 out F"{at[i]%%-35s}  {a[i]}"


if(!el) end ERR_INIT

if(&aURL) aURL=0
if(&aText) aText=0
if(&aElem) aElem=0

MSHTML.IHTMLElementCollection links
MSHTML.IHTMLElement link
int src=flags&3
sel src
	case 0 links=el.all.tags("A") ;;info: gets <a> without href
	case 1 links=el.document.links ;;info: does not get <a> without href
	case 2 link=el; goto g1
	case 3 end "flag 3"

foreach link links
	 g1
	str href
	if &aURL or src!1
		href=link.getAttribute("href" 0); err continue
		if(!href.len) continue
	if(&aURL) aURL[]=href
	if(&aText) aText[]=link.innerText; err aText[]=_s
	if(&aElem) aElem[]=link

err+ end _error

 note: tried to add flag 3 to get links from selection, but it is difficult and unreliable, works not with all pages.
