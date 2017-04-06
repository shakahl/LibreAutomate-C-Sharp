int w=win("Catkeys" "*.Window.*")
Acc a.Find(w "PAGETAB" "Find" "class=WindowsForms10.Window.8.app.0.2bf8098_r9_ad1" 0x1005)
a.DoDefaultAction
a.Navigate("parent")
a.Role(_s); out _s; out a.Name

 int w1=win("Catkeys" "*.Window.*")
 PF
 Acc a.Find(w1 "PAGETABLIST" "Tab group 1" "class=WindowsForms10.Window.*" 0x1005)
 PN
  a.Mouse(1)
 ARRAY(Acc) b; int i
 a.ObjectSelected(b)
 PN;PO
 for i 0 b.len; out b[i].Name

 int w=win("Catkeys" "*.Window.*")
 act w
 Acc a.Find(w "CLIENT" "Panels" "class=WindowsForms10.Window.8.app.0.2bf8098_r9_ad1" 0x1005)
 a.ObjectFocused
 out a.Name
 a.Mouse

