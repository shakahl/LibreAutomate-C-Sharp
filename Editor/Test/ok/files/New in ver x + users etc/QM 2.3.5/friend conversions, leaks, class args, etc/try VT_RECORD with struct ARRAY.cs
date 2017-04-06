 Currently for ARRAY(struct) we don't add a vartype neither a recordinfo.
 It is correct. MSDN even gives a sample code for it.
 Tried to use VT_RECORD instead. Either don't set IRecordingo, or change flags/vt in SAFEARRAY.
 This works, but I don't know whether it is correct. With some componants may not work.

out
SAFEARRAYBOUND sab.cElements=2
SAFEARRAY* a aa

 a=SafeArrayCreate(VT_RECORD 1 &sab)
 a=SafeArrayCreateEx(VT_RECORD 1 &sab 0)
WINAPI.SafeArrayAllocDescriptorEx(VT_RECORD 1 &a) ;;works
 WINAPI.SafeArrayAllocDescriptorEx(VT_I4 1 &a)
 WINAPI.SafeArrayAllocDescriptor(1 &a)
out a
a.rgsabound[0].cElements=2
a.cbElements=16
out SafeArrayAllocData(a)
 a.fFeatures=FADF_HAVEVARTYPE; int* pp=a; pp[-1]=VT_RECORD ;;works too

 a=SafeArrayCreateVector(VT_I4 0 100) ;;allocates data after descriptor

out _s.getstruct(*a 1)

word vt; if(!SafeArrayGetVartype(a &vt)) out "vt=%i" vt; else out "SafeArrayGetVartype failed"

 out LocalSize(a-16)
out "-------"

 if(!SafeArrayCopy(a &aa)) out _s.getstruct(*aa 1); else out "SafeArrayCopy failed"

 out SafeArrayDestroy(a)
 out LocalSize(a-16)
 ret

 IRecordInfo ri
 out SafeArrayGetRecordInfo(a &ri); out "ri=%i" ri
 
 ARRAY(str) as.create(1)
 if(!SafeArrayGetRecordInfo(as.psa &ri))
	  out ri
	 out SafeArraySetRecordInfo(a ri)
 ri=0; out SafeArrayGetRecordInfo(a &ri); out "ri=%i" ri
outx a.fFeatures
word vt2; if(!SafeArrayGetVartype(a &vt2)) out vt2; else out "SafeArrayGetVartype failed"
int* p=a-16; out "%i %i %i %i" p[0] p[1] p[2] p[3]
