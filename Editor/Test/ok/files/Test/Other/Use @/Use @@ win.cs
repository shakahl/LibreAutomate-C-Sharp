 act "alcula"
 act "+Scicalc"
 act "Calc[]Notepad"
 act "+sciCalc[]+Notepad"
 act "$^U.+ed .+pad$"

int h
 h=win("alcula")
 h=win("" "Scicalc")
 h=win("alcula" "Scicalc")
 h=win("alcula" "Scicalc" "calc")
 h=win("" "" "calc")
 h=win("*alculator" "" "" 1)
 h=win("*Alculat*" "" "" 3)
 h=win("") ;;first visible
 h=win("*" "not*" "" 1|0x800)
 h=win("" "noT*" "" 0x800)
 h=win("**" "" "" 1)
 h=win("**" "not*" "" 1|0x800)
 h=win("N" "" "+notepad" 32)
 h=win("N" "" "epad" 32)
 h=win("$^U.+pad$")
 h=win("Notepad[]Calc" "" "" 16)
 h=win("*Notepad[]Calc*" "" "" 16|1)
 h=win("$U.+Notepad[]Calc*" "" "" 16)
 h=win("$U.+Notepad[]Calc*" "" "" 16|1)
 h=win("NotepaD[]CalC" "" "" 16|2)
 h=win("*Notepad[]Calc*" "" "" 16|1)
act h

