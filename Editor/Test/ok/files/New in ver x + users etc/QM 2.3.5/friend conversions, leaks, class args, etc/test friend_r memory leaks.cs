 1
dll user32 [IsWindow]API_BSTR BSTR'b
dll user32 [IsWindow]API_ARRAY ARRAY(BSTR)a
dll user32 [IsWindow]API_Interface IUnknown'u
dll user32 [IsWindow]API_InterfaceX IXml'u
dll user32 [MoveWindow]API_VARIANT hWnd `v bRepaint
dll msvcrt [fabs]API_DATE DATE'd
dll user32 [MoveWindow]API_Class hWnd Acc'a x y bRepaint
dll user32 [IsWindow]API_Class2 __GlobalMem'u
type T3 !a !b !c
dll user32 [IsWindow]API_T3 T3't
 dll user32 [MoveWindow]API_str hWnd ~s bRepaint ;;error

 BSTR b="2000.09.09 05:05:05"
 VARIANT v="2000.09.09 05:05:05"
BSTR ba="one[]two[]three"
VARIANT va="one[]two[]three"

rep 100000
 rep 5
	 DATE d="2000.09.09 05:05:05"
	 DATE d=b
	 DATE d=v
	 API_DATE b
	 API_DATE v
	 DATE d=RetBstr(1)
	 DATE d=RetVariant(2)
	 DATE d=RetStr(1)
	 API_DATE RetBstr(1)
	 API_DATE RetVariant(2)
	 API_DATE RetStr(1)
	
	 ARRAY(str) a="one[]two[]three"
	 ARRAY(str) a=_s.from("one[]two[]three")
	 ARRAY(str) a=ba
	 ARRAY(str) a=va
	ARRAY(BSTR) a="one[]two[]three"
	 ARRAY(BSTR) a=ba
	 ARRAY(BSTR) a=va
	API_ARRAY a
	API_ARRAY ba
	API_ARRAY va
	va=ba; API_ARRAY va
	 VARIANT vai=5; API_ARRAY vai ;;type mismatch, OK
	 ARRAY(str) a=RetBstr(1)
	 ARRAY(str) a=RetVariant(2)
	 ARRAY(str) a=RetStr(1)
	API_ARRAY RetBstr(0)
	API_ARRAY RetVariant(1)
	API_ARRAY RetStr(0)
	
	 ARRAY(byte) ab; a=ab
	
	 ARRAY(str) d=Function236
	 out d
	 IXml x=Function236
	 IW Function236
	 BSTR b=Function236
	 out b
	
	 VARIANT v=b
	 VARIANT v=Function236
	 API_VARIANT 0 b 0
	 API_VARIANT 0 Function236 0
	
	 IXml x=CreateXml
	  outref x
	  API_InterfaceX x
	  API_Interface x
	 API_InterfaceX CreateXml
	 API_Interface CreateXml
	  outref x
	
	 Acc a.FromWindow(_hwndqm); API_Class 0 a 0 0 0
	 API_Class 0 acc(_hwndqm) 0 0 0
	 __GlobalMem m=RetClass; API_Class2 m
	 API_Class2 RetClass
	
	 out sizeof(T3)
	 T3 t
	 API_T3 t

 IUnknown u
 VARIANT d=u
