 str s.getclip("HTML Format")
str s.getsel(0 "HTML Format")
 out s
int i=find(s "<html" 0 1); if(i<0) ret
s.get(s i)
 out s; ret

str m

HtmlDoc d.InitFromText(s)
ARRAY(MSHTML.IHTMLElement) a
d.GetHtmlElements(a "img")
for i 0 a.len
	MSHTML.IHTMLElement e=a[i]
	s=e.getAttribute("src" 0)
	 out s
	str name.getfilename(s 1) ;;or try to get title or alt attribute
	m.formata("%s :out ''%s''[]" name s)

 out m
DynamicMenu m
