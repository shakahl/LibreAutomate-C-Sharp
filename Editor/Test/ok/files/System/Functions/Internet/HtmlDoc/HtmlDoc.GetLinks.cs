function ARRAY(MSHTML.IHTMLElement)&a

 Gets all links.

 REMARKS
 Links are A elements that have href attribute.
 To get all links in certain element, instead use GetHtmlElements with tag A. However it also returns A elements that don't have href attribute, eg anchors (link targets within the page).

 EXAMPLE
 HtmlDoc d
 d.SetOptions(2)
 d.InitFromWeb("http://www.quickmacros.com")
 ARRAY(MSHTML.IHTMLElement) a; int i
 d.GetLinks(a)
 for i 0 a.len
	 out a[i].innerText
	 out a[i].outerHTML
	 out a[i].getAttribute("href" 0)
	 out "-------"


MSHTML.IHTMLElementCollection c
c=d.links

a.redim(c.length)
for(_i 0 a.len) a[_i]=c.item(_i)

err+ end _error
