int w=wait(2 WV win("Mozilla Firefox" "Mozilla*WindowClass" "" 0x804))
Acc a1.Find(w "DOCUMENT" "" "" 0x3010 2)
str s
a1.WebPageProp(0 0 s) ;;get page HTML
;out s

type NAMEURL str'name str'url
ARRAY(NAMEURL) m ;;array for menu display names and URLs
int i
str sm
MenuPopup p

HtmlDoc d.InitFromText(s)
ARRAY(MSHTML.IHTMLElement) a
d.GetHtmlElements(a "img")
for i 0 a.len
	MSHTML.IHTMLElement e=a[i]
	s=e.getAttribute("src" 0); err continue
	 out s
	
	NAMEURL& r=m[] ;;add array element
	r.name.getfilename(s 1) ;;or try to get title or alt attribute
	r.url=s
	p.AddItems(r.name i+1) ;;add menu item

i=p.Show-1; if(i<0) ret ;;show menu

s=m[i].url
out s
