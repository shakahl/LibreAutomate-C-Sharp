out
#compile "__CArg"

 ARRAY(CArg) a.create(1)
 VARIANT v=a
  VARIANT vv=&v
  VARIANT vv.vt=VT_VARIANT|VT_BYREF; vv.pvarVal=&v
  VariantClear &vv


 typelib Project1 "Q:\Projects\from_notebook\VbActiveXdll\VbDll.dll"
 Project1.Class1 x._create
 VARIANT v=x.RetStruct
 outx v.vt
  out v.pRecInfo
 IRecordInfo& ri=+&v.pRecInfo
  out ri
 VARIANT vv=v
 outref ri


 ARRAY(CArg) a.create(1)
ARRAY(__Handle) a.create(1)
IRecordInfo ri; SafeArrayGetRecordInfo a.psa &ri;; ri.Release
a=0
 outref ri
 CArg* x=q_calloc(1 sizeof(CArg))
 __Handle* x=q_calloc(1 sizeof(__Handle))
__Handle* x=CoTaskMemAlloc(4); *x=0
 out LocalSize(x)
 out q_msize(x)
out "-----"
VARIANT v.vt=VT_RECORD; v.pvRecord=x; v.pRecInfo=ri
VariantClear &v
 outx v.vt; v.vt=0
 ri.RecordDestroy(x)
 out LocalSize(x)
 out q_msize(x)
out "-----"
