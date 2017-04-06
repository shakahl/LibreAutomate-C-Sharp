int w1=win("Untitled - Notepad" "Notepad")
SetProp(w1 "myluckynumber" 7)
 ...
w1=win("Untitled - Notepad" "Notepad")
int i=GetProp(w1 "myluckynumber")
out i
