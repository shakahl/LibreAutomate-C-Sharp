 /DEX_Main2
function hDlg hsb

int w1(70) w2(140) w3(-1)
SendMessage hsb SB_SETPARTS 3 &w1

 str p0="part 0"
 str p1="part 1"
 str sp.all(1000 2 32)
 p0+sp
 p1+sp
 SendMessage hsb SB_SETTEXTA 0 p0
 SendMessage hsb SB_SETTEXTA 1 p1
 
 __Hicon-- hi2=GetFileIcon("$qm$\run.ico")
 SendMessage(hsb SB_SETICON 2 hi2)
 
 SendMessage(hsb SB_SETTIPTEXT 0 "test")
 SendMessage(hsb SB_SETTIPTEXT 1 "test1")
 SendMessage(hsb SB_SETTIPTEXT 2 "test2")
