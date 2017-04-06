ARRAY(str) a
ARRAY(BSTR) b

 b=a
 __Handle h; b=h.Array(a)

 #opt dispatch 1
typelib Project1 "Q:\Projects\from_notebook\VbActiveXdll\VbDll.dll"
 Project1.Class1 x._create
 IDispatch x._create("Lietuviškas.Class1")

 interface# IClass1 :IDispatch
	 ARRAY'RetArray(ARRAY(byte)*ab)
	 {FC14C79C-FB58-412A-9D8E-445FCDCB01B4}

 interface@ IClass1 :IDispatch
	 ARRAY'RetArray(ARRAY(byte)*ab)
	 {FC14C79C-FB58-412A-9D8E-445FCDCB01B4}

interface# IClass1 :IDispatch
	ARRAY(BSTR)RetArray(ARRAY(byte)*ab)
	{FC14C79C-FB58-412A-9D8E-445FCDCB01B4}

IClass1 x._create("Lietuviškas.Class1")

ARRAY(byte) ab
b=x.RetArray(ab)
out b
