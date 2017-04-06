HtmlDoc d.InitFromInternetExplorer(win("" "IEFrame"))
ARRAY(MSHTML.IHTMLElement) a
d.GetLinks(a)
int i
str su sa sn
for i 0 a.len
	su=a[i].getAttribute("href" 2)
	if(!su.beg("/profile.php?id=")) continue
	sn=a[i].innerText
	if(!sn.len) continue ;;image
	sa.formata("%s, http://www.facebook.com%s[]" sn su)

str sf="$desktop$\facebook friends.txt"
sa.setfile(sf)
run sf
