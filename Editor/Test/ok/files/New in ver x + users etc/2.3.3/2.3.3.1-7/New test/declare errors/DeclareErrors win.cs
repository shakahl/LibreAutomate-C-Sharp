act "ddd"; act win("dd"); act child(""); act id(7)
 clo " "; hid " "; ont " "; mov 0 0 " "; siz 0 0 " "
 but id(1 "w"); but 1 "w"; but "name" "w"; but 1; but "jjj"
 men "" ""; men 1 ""; men 1; _i=men("" ""); _i=men(1 "w")
 _s.setwintext("ddd")
 min " "; max ""; res ""; _i=min("")
 but 1 ""; _i=but(1); but+ 1; but- 1; but* 1; but% 1
 _i=win; _i=win(1 1); _i=win(""); _i=win("" "" "" 0); _i=win("" "" "" 0 0 0); _i=wintest(_i "")
 _i=child; _i=child(1 1 ""); _i=child("" "" "f"); _i=child("" "" "" 0); _i=child("" "" "" 0 0 0); _i=childtest(_i "")

 acc; acc 1 1 "w"; acc "" "" "w"
 htm "" ""; htm "" "" "" "w"
 scan ""; scan "" "win"; share; share "win"; xm; xm 0; xm 0 "win"
 _s.getwintext(_i); _s.setwintext(_i); _s.getwinclass(_i); _s.getwinexe(_i)


  no error if window not found
 ifa("fffff") out 1
 web "" 0 "hh"
 run "" "" "" "" 0 "kk"
