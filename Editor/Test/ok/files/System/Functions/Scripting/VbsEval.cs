 /
function` $expression

 Evaluates VBScript expression and returns result.
 Expression can include functions and module-level variables that have been added with VbsAddCode.

 See also: <VbsAddCode>, <Scripting help>

 EXAMPLES
 out VbsEval("10*(2+5)")

 VbsAddCode "dim v"
 VbsExec "v=8"
 out VbsEval("v*2")


MSScript.ScriptControl- _vbs
VbsInit _vbs "VBScript"; err end _error

ret _vbs.Eval(expression)
err end VbsError(_vbs)
