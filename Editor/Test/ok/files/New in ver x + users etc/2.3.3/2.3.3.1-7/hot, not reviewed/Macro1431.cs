act "Microsoft Excel"
spe 10
key CH ;;Ctrl+Home to select A:1
str s s2
rep 5
	 get cell A
	s.getsel
	s.trim
	out s
	
	 set cell C
	key RR ;;Rigt Right
	s2="OK"
	s2.setsel
	
	 next row
	key HD ;;Home Down
