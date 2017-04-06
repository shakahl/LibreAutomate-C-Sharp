int w=wait(3 WV win(" - YouTube - Google Chrome" "Chrome_WidgetWin_1"))

 find the nearest common ancestor object, not DOCUMENT
Acc aParent.Find(w "STATICTEXT" "Links" "" 0x3001 3 0 "parent next")

ARRAY(Acc) aa; int i
ARRAY(str) aURL
 aParent.GetChildObjects(aa -1 "LINK" "^\S+ (Facebook|Twitter|Twitch|Instagram)$" "" 2) ;;by link name
aParent.GetChildObjects(aa -1 "LINK" "^\w+://www\.(facebook|twitter|twitch)\." "" 2) ;;by URL. Here it works because URL is in object Name, which is rare, usually URL is Value.
for i 0 aa.len
	Acc& r=aa[i]
	str name=r.Name
	str url=r.Value
	out "%-35s %s" aa[i].Name aa[i].Value
	aURL[]=url
