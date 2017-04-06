function` $name [`a1] [`a2] [`a3] [`a4] [`a5] [`a6] [`a7] [`a8] [`a9] [`a10]

 Calls a static function.

 name - class and function name, like Class1.StaticFunction1.
   Can be used wildcard character * as class or function name to call first matching function.


if(!x) end ERR_INIT

int vt=VT_VARIANT
SAFEARRAY sa.cbElements=16; sa.cDims=1
sa.fFeatures=FADF_VARIANT|FADF_HAVEVARTYPE
sa.rgsabound[0].cElements=getopt(nargs)-1
sa.pvData=&a1
 ret
ret x.Call(name +&sa)
err end _error
