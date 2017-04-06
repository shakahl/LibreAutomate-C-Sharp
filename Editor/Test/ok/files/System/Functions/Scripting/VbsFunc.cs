 /
function` $func [`a1] [`a2] [`a3] [`a4] [`a5] [`a6] [`a7] [`a8] [`a9] [`a10]

 Calls a VBScript function added with VbsAddCode.

 func - function name.
 a1, a2, ... - arguments.

 See also: <VbsAddCode>, <Scripting help>

 EXAMPLES
 VbsAddCode "$desktop$\test.vbs" 2 ;;get VBScript code from file
 out VbsFunc("Func1")
 out VbsFunc("Func2" 1 2.5 "arg3")

 VbsAddCode "" ;;get VBScript code from this macro
 int R=VbsFunc("Add" 1 2)
 out R
 #ret
 function Add(byval a, byval b)
 Add=a+b
 end function


MSScript.ScriptControl- _vbs
if(!_vbs) end "code not added"

RECT r.bottom=VT_VARIANT
SAFEARRAY sa.cbElements=16; sa.cDims=1
sa.fFeatures=FADF_VARIANT|FADF_HAVEVARTYPE|FADF_AUTO|FADF_FIXEDSIZE
sa.rgsabound[0].cElements=getopt(nargs)-1
sa.pvData=&a1
SAFEARRAY* psa=&sa

ret _vbs.Run(func +&psa)
err end VbsError(_vbs)
