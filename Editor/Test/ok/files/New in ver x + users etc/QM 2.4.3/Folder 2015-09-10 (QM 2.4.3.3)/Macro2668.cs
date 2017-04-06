str s.getmacro("Scripting C#, call any functions2")
 rep(3) s+s
 s.fix(s.len/2)
 out s
out findrx(s "(?mi)(\n|.)*? YGY")

 sel s 4
	 case "$(?mi)X(\n|\r|.)*? Y"
	 out 1
	 
	 case "$.+"
	 out 2
	 
	 case else
	 out 0
 out "etc"

s.replacerx("(?mi)(\n|.)*? YGY" "k")
out s
