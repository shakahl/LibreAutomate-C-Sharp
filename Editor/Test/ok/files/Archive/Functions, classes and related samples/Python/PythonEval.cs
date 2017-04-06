 /
function'VARIANT $expression

 Evaluates Python expression and returns result.

 See also: <VbsEval>, <Python scripting help>.

 EXAMPLE
 int v=PythonEval("3*3")
 out v


#opt nowarnings 1

MSScript.ScriptControl- _python
VbsInit _python "Python"; err end _error

VARIANT v=_python.Eval(expression); err end _error
if(_python.Error.Number) end VbsError(_python)
ret v
