web "http://www.quickmacros.com" 12|1
int w=wait(3 WV win("Internet Explorer" "IEFrame"))
act w
Acc a.Find(w "TEXT" "" "a:name=q" 0x3004 3)
a.SetValue("A")
1
a.Select(1)
key "B"
