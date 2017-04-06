typelib Project1 "Q:\Projects\from_notebook\VbActiveXdll\VbDll.dll"
 #opt dispatch 1
 Project1.Class1 x._create
IDispatch x._create("Lietuviškas.Class1")
 BSTR b
 ARRAY(BSTR) b
 VARIANT v=x.RetVariantEmpty(b)
 outx v.vt
 out v.lVal

CsScript xx.Init

 ARRAY(BSTR) a="bstr"
ARRAY(str) a="str"
 x.ArgVariant("test" -1)
 x.ArgVariant(a -1)
 x.ArgArray(a -1)
 x.ArgVariant(xx.x.RetArray -1)
 x.ArgArray(xx.x.RetArray -1)
