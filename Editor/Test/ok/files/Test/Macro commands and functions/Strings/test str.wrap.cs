ClearOutput
str s="first line first line first line first[][]second line second line second line second line"
 str s="first line []second line"
 str s="first line"
 str s="first line []first line "
 str s="first         []second line"
s.wrap(10 "" "" 0)
out s
out "-----"


 str s="one.two three,four"
 
 s.wrap(5 "" ": " 0)
 out s
