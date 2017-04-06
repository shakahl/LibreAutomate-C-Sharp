 out win("Untitled - Notepad" "Notepad" "NOTEPAD")
 act win("Not[]Calc" "" "" 0x10)
 act win(10 10)
 act win("Not" "" "" 0x0 10 10)
 int h=win("Calculator" "" "" 3)
 int h=win("Calculator" "SciCalc")
 int h=win("Calculator" "SciCalc" "cAlc")
 int h=win("calculator" "SciCalc" "calc" 3|128|256 0x14ca0044 0x50100)
 int h=win(".+culator" "SciCalc" "calc" 0x200)
 int h=win("Options" "" "Quick" 3|32)
 int h=win("Options" "" "" 3|64 _hwndqm 0)
 int h=win("Maxthon" "IEFrame" "Maxthon" 0x400)
 int h=win("Maxthon" "*Frame" "Maxthon" 0x800)
 int h=win("Maxthon" "IEFrame" "Maxthon" 0x1000)

 int h=win(10 10 1)

zw h
