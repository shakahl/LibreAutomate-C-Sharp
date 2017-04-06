out
HtmlDoc d.InitFromFile("$desktop$\html.txt")
 out d.GetHtml("TABLE" 10)
 MSHTML.IHTMLElement el=d.GetHtmlElement("A" 10)
MSHTML.IHTMLElement el=d.GetHtmlElement("" "sponsorLogo")
out el.outerHTML
