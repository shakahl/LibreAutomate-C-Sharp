typelib MSXML {D63E0CE2-A0A2-11D0-9C02-00C04FC99C8E} 2.0
#err 1
typelib MSXML {F5078F18-C551-11D3-89B9-0000F81FE221} 3.0

type QMFORUMRSSITEM str'title str'url str'text
ARRAY(QMFORUMRSSITEM) a

MSXML.XMLDocument xd._create
xd.url="http://www.quickmacros.com/forum/rss.php"
MSXML.IXMLElement e1 e2 e3
e1=xd.root.children.item("CHANNEL")
foreach e2 e1.children
	str tag=e2.tagName
	if(tag~"ITEM"=0) continue
	QMFORUMRSSITEM& r=a[a.redim(-1)]
	e3=e2.children.item("TITLE")
	r.title=e3.text
	e3=e2.children.item("LINK")
	r.url=e3.text
	e3=e2.children.item("DESCRIPTION")
	r.text=e3.text

 now all is in array, and you know what to do with it
int i
for i 0 a.len
	ShowText a[i].title a[i].text
