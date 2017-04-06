function` MSCOREE._MethodInfo'mi [`a1] [`a2] [`a3] [`a4] [`a5] [`a6] [`a7] [`a8] [`a9] [`a10]

 Calls a public static function.
 Returns its return value.

 name - class and function name, like Class1.StaticFunction1.
   Class name can be omitted or * (like "*.Func"). Function name also can be *. Then calls first matching function.
 a1-a10 - arguments.


if(!x) end ERR_INIT

int vt=VT_VARIANT
SAFEARRAY sa.cbElements=16; sa.cDims=1
sa.fFeatures=FADF_VARIANT|FADF_HAVEVARTYPE
sa.rgsabound[0].cElements=getopt(nargs)-1
sa.pvData=&a1

 ret mi.Invoke_3(0 +&sa)

SAFEARRAY* sap=&sa
ARRAY(VARIANT)& av=+&sap
ret mi.Invoke_3(0 av)

err+ end _error
