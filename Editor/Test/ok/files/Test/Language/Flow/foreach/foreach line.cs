ClearOutput
str s
str lines="line1[]line2[][]line3"
foreach s lines
	 if(!s.len) continue
	out s

out "---"
 out lines
 out "---"
