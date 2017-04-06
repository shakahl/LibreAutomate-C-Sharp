int w=wait(3 WV win(" – 118 - Mozilla Firefox" "MozillaWindowClass"))
act w
Acc a1.FindFF(w "INPUT" "" "id=ctl00_phMainContent_transFields_tbDepartureStation" 0x1004 3)
Acc a2.FindFF(w "INPUT" "" "id=ctl00_phMainContent_transFields_tbArrivalStation" 0x1004 3)
str s1 s2
s1=a1.Value
s2=a2.Value
a1.Select(1); key Ca (s2)
a2.Select(1); key Ca (s1)
key Y
