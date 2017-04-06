 int w=win("Catkeys" "*.Window.*")
 Acc a.Find(w "PAGETAB" "Find" "class=WindowsForms10.Window.8.app.0.2bf8098_r9_ad1" 0x1005)
 a.DoDefaultAction

int w=win("Catkeys" "*.Window.*")
Acc a.Find(w "SEPARATOR" "Splitter 10" "class=WindowsForms10.Window.8.app.0.2bf8098_r9_ad1" 0x1025)
a.Mouse(2)
