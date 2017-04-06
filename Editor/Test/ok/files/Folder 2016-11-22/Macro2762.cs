out
HtmlDoc d.InitFromWeb("http://www.quickmacros.com")

 str action data
 out d.GetForm(d.GetHtmlElement("form" 0) action data)
 out action
 out "-------"
 out data

 out d.GetText
 out d.GetText("p" 1)
 out d.GetHtml("form" 0)
 out d.GetHtml(0 d.GetHtmlElement("form" 0))

 ARRAY(MSHTML.IHTMLElement) a; int i
  d.GetHtmlElements(a "" "form" 0)
 d.GetHtmlElements(a "" "" d.GetHtmlElement("form" 0))
 for(i 0 a.len) out a[i].tagName
