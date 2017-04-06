int w=win("WordWeb" "TTheDi")
int c=child("" "TTabControl" w) ;;page tab list
SelectTab c 2
 SelectTab c "Type of"
 SelectTab c "Nearest"
 SelectTab c "Simi*"

 int w1=win("Options" "#32770")
 Acc a.Find(w1 "PAGETAB" "Files" "class=SysTabControl32" 0x1005)
 a.Select(3)

 int w1=win("Options" "bosa_sdm_Microsoft Office Word 11.0")
  act w1
 Acc a.Find(w1 "PAGETAB" "Edit" "class=bosa_sdm_Microsoft Office Word 11.0" 0x1005)
  a.Select(3)
 a.DoDefaultAction
