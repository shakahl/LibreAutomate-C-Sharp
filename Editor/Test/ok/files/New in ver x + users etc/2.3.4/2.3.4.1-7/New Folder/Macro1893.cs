 ---- Recorded 2012.12.17 11:35:38 ----
opt slowkeys 1; opt slowmouse 1; spe 100
lef 683 1043
int w1=act(win("Document1 - Microsoft Word" "OpusApp"))
lef+ 1159 276
lef- 1226 329
'X              ;; Delete
lef+ 1571 364
lef- 1451 360
'X              ;; Delete
lef 1177 87
int w2=wait(19 WV win("Edit" "MsoCommandBarPopup"))
lef 1192 109
lef 1171 88
wait 5 WV w2
lef 1202 135
lef 539 1042
int w3=act(win("Untitled - Notepad" "Notepad"))
dou 1234 503
'X              ;; Delete
opt slowkeys 0; opt slowmouse 0; spe -1
 --------------------------------------
