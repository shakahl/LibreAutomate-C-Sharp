out
HtmlDoc d.InitFromFile("$desktop$\html.txt")
 HtmlDoc d.InitFromWeb("http://www.quickmacros.com/index.html")
 str s=d.GetText
 str s=d.GetText("title")
 str s=d.GetText("body")
 str s=d.GetText("A" 11)
 str s=d.GetHtml
 str s=d.GetHtml("body" 0 1)
 str s=d.GetHtml("html")
 str s=d.GetHtml("" 0 1)
 str s=d.GetHtml("head")
 str s=d.GetHtml("Table" 11 1)
str s=d.GetHtml("Table" 1 1)
out s
