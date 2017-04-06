spe 10
str s
rep
	act "Excel"
	s.getsel
	s.rtrim("[]")
	out s
	if(!s.len) break
	key D
	
	act "Notepad"
	s.setsel
	key Y
