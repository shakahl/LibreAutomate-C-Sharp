function$ ~namePart1 ~namePart2

lpstr r=m.Get(_s.from(namePart1 namePart2))
if(!r) goto ge
ret r

err+
	 ge
	end "the variable does not exist"
