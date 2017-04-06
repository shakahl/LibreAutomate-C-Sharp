out
 ----
int w=win("Options" "#32770")
act w
Acc a.Find(w "LIST" "Gray tray icon" "class=ListBox[]id=1285" 0x1005)
a.ObjectFocused(1)
 a.Focus
out a.Name

 ARRAY(Acc) x; int i
 a.ObjectSelected(x)
 for(i 0 x.len) out x[i].Name
