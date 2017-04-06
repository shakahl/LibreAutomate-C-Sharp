 int w=win("Catkeys" "*.Window.*")
 Acc a.Find(w "LISTITEM" "Functions" "class=WindowsForms10.Window.*" 0x1005)
int w=win("Catkeys" "*.Window.*")
Acc a.Find(w "LISTITEM" "FileDelete" "class=WindowsForms10.Window.*" 0x1005)
 a.DoDefaultAction
 a.Select(1)
 a.Select(2)
 a.Select(8)
 a.Select(16)
