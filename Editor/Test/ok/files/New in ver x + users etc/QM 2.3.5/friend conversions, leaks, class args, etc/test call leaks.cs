dll user32 [IsWindow]API_UNDEF ...
dll user32 [IsChild]API_UNDEF2 ... ;;2 param
dll user32 [MoveWindow]API_UNDEF6 ... ;;6 param
 dll "qm.exe" TestArr ...
#compile "__CArg"
 out
int+ g_noout=1

 rep 1000000
	 BSTR b="jjjjjjjjjjjjj"
	 call(&IsWindow b)
	 call(&IsWindow RetBstr)
	 call(&IsWindow SysAllocString(L"hhhhhhhhhhhhhhhnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnh"))
	 call(&IsWindow CreateXml)
	 call(&IsWindow RetClass)
	  out "BEGIN"
	 CArg m; call(&IsChild m)
	 CArg mm=RetCArg
	 RetCArg
	 call(&IsChild RetCArg)
	 CArg mmm; ArgCArg mmm
	 ArgCArg RetCArg
	  out "END"
	
	 BSTR b="jjjjjjjjjjjjj"
	 API_UNDEF b
	 API_UNDEF RetBstr
	 API_UNDEF SysAllocString(L"hhhhhhhhhhhhhhhnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnh")
	 API_UNDEF CreateXml
	 API_UNDEF RetClass
	  out "BEGIN"
	 CArg m; API_UNDEF2 m
	 CArg mm=RetCArg
	 RetCArg
	 API_UNDEF2 RetCArg
	 CArg mmm; ArgCArg mmm
	 ArgCArg RetCArg
	  out "END"
	
	 ARRAY(BSTR) a.create(10)
	 VARIANT v="dksljkdlsjkfdhsjkd"
	  API_UNDEF2 a 5
	  API_UNDEF6 a v a v
	 TestArr 1 a 2
	 str s="str"; BSTR b="BSTR"
	 sprintf(_s.all(1000) "%i %s %S %g" 1 s b 10.25)
	 out _s.fix
	
