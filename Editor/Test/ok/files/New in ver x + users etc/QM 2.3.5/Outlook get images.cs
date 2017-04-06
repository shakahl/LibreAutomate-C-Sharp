int w=win("Inbox - Microsoft Outlook" "rctrl_renwnd32")
HtmlDoc d.InitFromInternetExplorer(w)
ARRAY(MSHTML.IHTMLElement) a
d.GetHtmlElements(a "IMG")
int i
for i 0 a.len
	str s=a[i].getAttribute("src" 0)
	sel s 3
		case ["*.jpg","*.jpeg"]
		out s
	