out
#compile "__CArg"
 CArg x

 rep 1000000
	 x.x._create
	 ArgCArg x ;;OK
	 y=x
	
	 x.x=0
	 x=RetCArg
	
	 ArgCArg RetCArg ;;OK

 out x.x
 outref x.x
