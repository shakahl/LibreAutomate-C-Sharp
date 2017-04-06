dll user32 [MoveWindow]API_VARIANT hWnd `v bRepaint
dll user32 [IsWindow]API_BSTR BSTR'b
out

  #opt dispatch 1
 typelib Project1 "Q:\Projects\from_notebook\VbActiveXdll\VbDll.dll"
  Project1.Class1 x._create
 IDispatch x._create("Lietuviškas.Class1")

rep 100000
	 API_BSTR 1111154545.65656
	 API_BSTR "strrrrrrrrrrrrrrrrrrrrr"
	 API_BSTR RetBstr
	 API_BSTR RetArrayBstr
	 API_BSTR RetArrayStr
	 API_BSTR RetVariant(1)
	 API_BSTR RetVariant(3)
	 API_BSTR x.RetStruct ;;error
	
	 x.ArgBstr(5555555.222)
	 x.ArgBstr("strrrrrrrrrrrrrrrrrrrrr")
	 x.ArgBstr(RetBstr)
	 x.ArgBstr(RetArrayBstr) ;;error if IDispactch, it's OK
	 x.ArgBstr(RetArrayStr) ;;error if IDispactch, it's OK
	 x.ArgBstr(RetVariant(1))
	 x.ArgBstr(RetVariant(3)) ;;error if IDispactch, it's OK
	
	 API_VARIANT 0 1111154545.65656 0
	 API_VARIANT 0 "strrrrrrrrrrrrrrrrrrrrr" 0
	 API_VARIANT 0 RetVariant(1) 0
	 API_VARIANT 0 RetVariant(3) 0
	 API_VARIANT 0 RetVariant(4) 0
	 API_VARIANT 0 x.RetStruct 0 ;;VT_RECORD
	 BSTR b="dggggggggggggggggggggggg"; API_VARIANT 0 b 0
	 API_VARIANT 0 RetBstr 0
	 API_VARIANT 0 RetInterface 0
	 API_VARIANT 0 RetArrayStr 0
	 API_VARIANT 0 RetArrayBstr 0
	
	 x.ArgVariant(1111154545.65656)
	 x.ArgVariant("strrrrrrrrrrrrrrrrrrrrr")
	 x.ArgVariant(RetVariant(1))
	 x.ArgVariant(RetVariant(3))
	 x.ArgVariant(RetVariant(4))
	 x.ArgVariant(x.RetStruct) ;;VT_RECORD
	 BSTR b="dggggggggggggggggggggggg"; x.ArgVariant(b)
	 x.ArgVariant(RetBstr)
	 x.ArgVariant(RetInterface)
	 x.ArgVariant(RetArrayStr)
	 x.ArgVariant(RetArrayBstr)
	
	 BSTR b="nnnnnnnnnnnnnnnnnnnnnn"
	  BSTR b
	 ArgBstr "sdsdsdssdsdsdsds" b RetBstr 5
	 ArgBstr "sdsdsdssdsdsdsds" b RetBstr 56565656.65656
	 BSTR c=RetBstr
	 ArgBstr1 RetBstr
	 ArgBstr1 b
	 ArgBstr b b RetBstr b
	 ArgBstr "sdsdsdssdsdsdsds" b b 56565656.65656
	 ArgBstr RetArrayBstr RetArrayStr RetVariant(2) RetVariant(3)
	
	VARIANT v="one[]two[]three"
	ARRAY(str) a=v
	ARRAY(str) aa=RetArrayStr
	ARRAY(str) aaa=RetArrayBstr
	ArgArrayStr v
	ArgArrayStr RetArrayStr
	ArgArrayStr RetArrayBstr
	
	 BSTR a b c
	 a="sssssssssssssssssssssssssssss"
	 b=" ffffffffffffffffffffffffffff"
	 c.add(a b)
	 c.add(a)
	 c.add(c)
	 c.add(c c)
	 c.add("ssssssssss" RetStr(1))
	 VARIANT v="gggggggg"; c.add(v RetVariant(2))
	 out c
	 rep(3) a.alloc(RandomInt(5 50))
	 out a.len
	 a.cmp("zzzzzzzzzz")
	 a.cmp(RetVariant(2))
	
	 DATE d.getclock
	  SYSTEMTIME st.wDay=1
	  d.add(st)
	 
	 double k=d.diff("2013.05.01")
	 BSTR b="2013.05.01"; double kk=d.diff(b)
	 double kkk=d.diff(RetVariant(2))
	 out k
	 out d
	
	 BSTR b="hjhjhjhkhjk"
	 VARIANT v.attach(b)
	 out v
	 out b
	
	 VARIANT v
	 v.add("one" "two")
	 v.add("three")
	 v.sub(5 6)
	 v.mul(5 6)
	 v.mul(2)
	 v.div(50 6)
	 v.round(v 2)
	 v.round
	 v.round(55.12345 2)
	 v.fix
	 v.fix(425.258)
	 _i=v.cmp("Onetwo" 1)
	 out _i
	 out v
	
	 ARRAY(str) a.create(10)
	 a[]="kkkkkkk"
	 a.insert(5)
	 a.redim(20)
	 a.remove(4)
	 a.shuffle
	 a.sort
	
	 RetVariant(2)
	 RetInterface
	
	 RetBstr(2)
	 ARRAY(str) a; ArgArrayStr a.create(5)
