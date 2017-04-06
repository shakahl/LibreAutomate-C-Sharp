 #opt dispatch 1
typelib Project1 "Q:\Projects\from_notebook\VbActiveXdll\VbDll.dll"
Project1.Class1 x._create
 IDispatch x._create("Lietuviškas.Class1")

 BSTR bb="test"
 x.ArgBstr(bb)

 VARIANT bb="test"
 x.ArgVariant(bb)

 ARRAY(BSTR) aa.create(1); aa[0]="test"
 x.ArgArray(aa)
 #ret

 x.DateArg(RetDate)

 rep 1000000

BSTR b="BSTR"
VARIANT v="VARIANT"
ARRAY(BSTR) a="ARRAY"
 ARRAY(str) a="ARRAY str"
 ARRAY(lpstr) a.create(1); a[0]="ARRAY lpstr"
DATE d.getclock
CURRENCY c="1000.11"
FLOAT f=10.5
 x.AllArgTypes(-1 b v a d c f 0)
 x.AllArgTypes(-1 "string" "varstring" a "2000.02.03 04:05" 11111111.5555 11111111.5555 0)
 ARRAY(byte) aa.create(1); aa[0]=5
 x.AllArgTypesBA("string" "varstring" aa "2000.02.03 04:05" 11111111.5555 11111111.5555)

rep 100000
	Acc ac.FromWindow(_hwndqm)
	x.ArgObject(ac.a)
	x.AllArgTypes(0 b v a d c f 0)
	x.AllArgTypes(0 b v a d c f ac.a)
	x.AllArgTypes(0 b v a d c f RetInterface)
