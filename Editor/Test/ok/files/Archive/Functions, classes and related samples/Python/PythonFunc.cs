 /
function'VARIANT $func [VARIANT'a1] [VARIANT'a2] [VARIANT'a3] [VARIANT'a4] [VARIANT'a5] [VARIANT'a6] [VARIANT'a7] [VARIANT'a8] [VARIANT'a9] [VARIANT'a10]

 Calls Python user-defined function that has been added with PythonAddCode.

 See also: <PythonAddCode>, <VbsFunc>, <Python scripting help>.

 EXAMPLE
 PythonAddCode ""
 int v=PythonFunc("Add" 2 3)
 out v
 
 #ret
 def Add(a, b):
 	return a+b


#opt nowarnings 1

MSScript.ScriptControl- _python
if(!_python) end "code not added"

RECT r.bottom=VT_VARIANT
SAFEARRAY sa.cbElements=16; sa.cDims=1
sa.fFeatures=FADF_VARIANT|FADF_HAVEVARTYPE|FADF_AUTO|FADF_FIXEDSIZE
sa.rgsabound[0].cElements=getopt(nargs)-1
sa.pvData=&a1
SAFEARRAY* psa=&sa

VARIANT v=_python.Run(func +&psa); err end _error
if(_python.Error.Number) end VbsError(_python)
ret v
