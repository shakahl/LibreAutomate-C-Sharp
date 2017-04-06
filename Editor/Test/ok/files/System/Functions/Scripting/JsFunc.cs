 /
function` $func [`a1] [`a2] [`a3] [`a4] [`a5] [`a6] [`a7] [`a8] [`a9] [`a10]

 Calls a JScript function added with JsAddCode.

 func - function name.
 a1, a2, ... - arguments.

 See also: <JsAddCode>, <VbsFunc> (examples), <Scripting help>


MSScript.ScriptControl- _js
if(!_js) end "code not added"

RECT r.bottom=VT_VARIANT
SAFEARRAY sa.cbElements=16; sa.cDims=1
sa.fFeatures=FADF_VARIANT|FADF_HAVEVARTYPE|FADF_AUTO|FADF_FIXEDSIZE
sa.rgsabound[0].cElements=getopt(nargs)-1
sa.pvData=&a1
SAFEARRAY* psa=&sa

ret _js.Run(func +&psa)
err end VbsError(_js)
