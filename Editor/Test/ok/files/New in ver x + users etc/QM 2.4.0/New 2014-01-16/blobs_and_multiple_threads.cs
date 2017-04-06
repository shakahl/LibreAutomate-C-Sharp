out
QMITEM q
rep
	int i=0
	rep
		 i=qmitem(-i)
		i=qmitem(-i 0 &q 1|2|4|8|32|64|128)
		if(i=0) break
		 q.text.setmacro(1)
		 0.001
		 out "%-30s %-30s %s" q.name q.trigger q.programs
