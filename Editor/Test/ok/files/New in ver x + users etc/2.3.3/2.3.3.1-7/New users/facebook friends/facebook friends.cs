MSHTML.IHTMLDocument2 d=htm(win("" "IEFrame"))
MSHTML.IHTMLElement e
str su sa sn
foreach e d.links
	su=e.getAttribute("href" 2)
	out su ;;this line displays link URL in QM output. It displays all links, not only friends. Edit the following code so that it would get URLs that you need.
	if(!su.beg("/profile.php?id=")) continue
	sn=e.innerText
	if(!sn.len) continue ;;image
	sa.formata("%s, http://www.facebook.com%s[]" sn su)

str sf="$desktop$\facebook friends.txt"
sa.setfile(sf)
 run sf ;;later uncomment this
