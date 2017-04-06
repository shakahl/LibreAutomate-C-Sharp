int w=act(win("" "Chrome_WidgetWin_1"))
Acc a.Find(w "DOCUMENT" "" "" 0x3010 2)
ARRAY(Acc) aa; int i
ARRAY(str) aURL
a.GetChildObjects(aa -1 "LINK" "Facebook|Twitter|Twitch|Instagram" "" 2)
for i 0 aa.len
	Acc& r=aa[i]
	str name=r.Name
	str url=r.Value
	out "%-35s %s" aa[i].Name aa[i].Value
	aURL[]=url

 int w1=act(win("test - Editor" "Notepad"))
 paste aURL
 
 for i 0 aURL.len
	 run aURL[i]
