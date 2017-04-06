int h=win("Avant")
 int h=win("Internet Explorer")
str s

 web "" 0 s h
 out s
SHDocVw.IWebBrowser2 b=web("" 0 h)
MSHTML.IHTMLDocument2 docb=b.Document
out docb.url

MSHTML.IHTMLDocument2 doc=htm(h)
out doc.url
out "------"
