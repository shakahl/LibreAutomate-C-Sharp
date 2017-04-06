int w=win("- Google Chrome" "Chrome_WidgetWin_1")
 Acc a.Find(w "PUSHBUTTON" "" "" 0x3001 0)
Acc a.Find(w "DOCUMENT" "" "" 0x3001 0)
out a.Name
