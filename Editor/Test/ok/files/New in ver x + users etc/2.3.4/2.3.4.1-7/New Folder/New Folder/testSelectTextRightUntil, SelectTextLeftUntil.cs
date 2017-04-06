out 
 act "Word"
 act "Notepad"
 act "OpenOffice"

_s=SelectTextRightUntil("]" 0)
 _s=SelectTextRightUntil("X" 0)
 _s=SelectTextRightUntil("X" 8)
 _s=SelectTextRightUntil("ą" 8)
 _s=SelectTextRightUntil("''" 4)
 _s=SelectTextRightUntil("X" 4)
 _s=SelectTextRightUntil("notfound[8]")
 _s=SelectTextRightUntil("notfound[8]" 16)
 _s=SelectTextRightUntil("[\[\]]" 2)
 _s=SelectTextRightUntil("[''[39]]" 2|4) ;;search for closing " or ' character, assuming current position is before the opening " or ' character

 _s=SelectTextLeftUntil("[" 0)
 _s=SelectTextLeftUntil("o" 0)
 _s=SelectTextLeftUntil("ol" 8)
 _s=SelectTextLeftUntil("ą" 0)
 _s=SelectTextLeftUntil("X" 4)
 _s=SelectTextLeftUntil("''" 4)
 _s=SelectTextLeftUntil("notfound[8]")
 _s=SelectTextLeftUntil("notfound[8]" 16)
 _s=SelectTextLeftUntil("[\[\]]" 2|4)
out "'%s'" _s
 1
 key L

#ret
She almost felt dizzy at [delete ‘at’ replace with ‘from’] the flood of relief that [delete ‘that’]
humans were not involvedzzzzzzz, but they had XbeeXnbbX next [delete 'had been' replace with ‘were’] a target.
aaa zzz Xąčę "bbb ccc" ddd
