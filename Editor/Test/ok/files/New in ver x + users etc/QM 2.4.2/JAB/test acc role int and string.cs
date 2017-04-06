out
int w=win("Options" "#32770")
Acc a.Find(w "PUSHBUttON" "Debug..." "class=Button" 0x1005)
 Acc a.Find(w "B" "Run at startup" "class=Button" 0x1005)
 Acc a=acc("Debug..." ROLE_SYSTEM_PUSHBUTTON w "Button" "" 0x1005)
 Acc a=acc("Debug..." "PUSHBUTTON" w "Button" "" 0x1005)
 Acc a=acc("Debug..." "B" w "Button" "" 0x1005)
Acc a=acc("Debug..." "push BUtton" w "Button" "" 0x1005)
out a.Name
str s1 s2
a.Role(s1 s2)
out "%s %s" s1 s2
