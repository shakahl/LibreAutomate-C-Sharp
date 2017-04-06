out
 int w=win("" "" "" 0 "cid=15")
 int w=win("" "" "" 0 "cid=15" 2)
 int w=win("" "" "" 0 "ctext=O" 4)
 int w=win("" "" "" 0 "cclass=edit" 4)
 int w=win("" "" "" 0 "cclass=edit[]ctext=o[]cid=5" 2)
 int w=wait(0 WV win("" "" "" 0 "cclass=edit[]ctext=o[]cid=5" 2))

 int w=win("Dialog" "#32770")
 w=child("" "" w 0 "ctext=k")
 w=child("" "" w 0 "cid=1000")
 w=wait(0 WC child("" "" w 0 "ctext=k"))
 w=child(5 "" "" w)

 SetProp win("Untitled - Notepad" "Notepad") "qm_test" 5; out
 out w
 int w=win("" "Notepad" "" 0 "getprop=qm_test 5")

 SetProp id(15 win("" "Notepad")) "qm_test" 5; out
  out w
 int w=child("" "" "Notepad" 0 "getprop=qm_test 5")

int w=win("Font" "#32770")
w=child("" "ComboLBox" w 0x0 "accName=*style:")

outw w
if(!w) ret
RECT r; GetWindowRect w &r
__OnScreenRect osr.Show(0 r)
1
 out wintest(w "" "" "" 0 "cclass=edit[]ctext=o[]cid=5")
 out childtest(w "" "" 0 0 "cclass=edit")
