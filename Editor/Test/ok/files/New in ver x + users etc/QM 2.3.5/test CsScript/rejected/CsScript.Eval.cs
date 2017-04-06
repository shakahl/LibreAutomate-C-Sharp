function` [`a1] [`a2] [`a3] [`a4] [`a5] [`a6] [`a7] [`a8] [`a9] [`a10]


if(!x) end ERR_INIT

int vt=VT_VARIANT
SAFEARRAY sa.cbElements=16; sa.cDims=1
sa.fFeatures=FADF_VARIANT|FADF_HAVEVARTYPE
sa.rgsabound[0].cElements=getopt(nargs)
sa.pvData=&a1
ret x.Eval(&sa)
err end _error

 function` $code [`a1] [`a2] [`a3] [`a4] [`a5] [`a6] [`a7] [`a8] [`a9] [`a10]
 
 
 if(!x) end ERR_INIT
 
 int vt=VT_VARIANT
 SAFEARRAY sa.cbElements=16; sa.cDims=1
 sa.fFeatures=FADF_VARIANT|FADF_HAVEVARTYPE
 sa.rgsabound[0].cElements=getopt(nargs)-1
 sa.pvData=&a1
 ret x.Eval(code &sa)
 err end _error
