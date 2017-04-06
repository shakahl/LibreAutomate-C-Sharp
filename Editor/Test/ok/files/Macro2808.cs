int w=win("Catkeys" "*.Window.*")
Acc a.Find(w "LIST" "Files" "class=WindowsForms10.Window.8.app.0.5c39d4_r9_ad1" 0x1005)
 a.ObjectFocused(1)
 a.ObjectSelected()
 a.ObjectFromPoint(270 300 1|2)
a.ObjectFromPoint(100 50 1)
out a.Name
