out
str s="one two, three"

 out findw(s "two")
 out findw(s "two" 0 " ")
 out findw(s "two" 0 " ,")
 out findw(s "two" 0 ",")
 out findw(s "two" 0 "," 0x200)
 out findw(s "Two" 0 "" 1)
 out findw(s "two, " 0 "" 0)
 out findw(s "two, " 0 "" 64)
