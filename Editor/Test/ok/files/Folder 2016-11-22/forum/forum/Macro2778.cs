int w=wait(3 WV win("New Tab - Google Chrome" "Chrome_WidgetWin_1"))
act w
Acc a.Find(w "STATICTEXT" "Search Google or type URL" "" 0x3001 3)
a.Select(1)
str s="Some value"
key (s)
 or
 paste s
