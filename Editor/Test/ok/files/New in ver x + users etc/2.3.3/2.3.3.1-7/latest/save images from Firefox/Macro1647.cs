int w=wait(2 WV win("Mozilla Firefox" "Mozilla*WindowClass" "" 0x804))
Acc a1.Find(w "DOCUMENT" "" "" 0x3010 2)
str s
a1.WebPageProp(0 0 s)
 out s

int i
HtmlDoc d
d.SetOptions(2)
d.InitFromText(s)
ARRAY(MSHTML.IHTMLElement) a
d.GetHtmlElements(a "img")
for i 0 a.len
	MSHTML.IHTMLElement e=a[i]
	s=e.getAttribute("src" 0)
	if(s.beg("about:")) s.get
	out s
