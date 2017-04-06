 /
function` $expression

 Evaluates JScript expression and returns result.
 Expression can include functions and module-level variables that have been added with JsAddCode.

 See also: <JsAddCode>, <VbsEval> (examples), <Scripting help>


MSScript.ScriptControl- _js
VbsInit _js "JScript"; err end _error

ret _js.Eval(expression)
err end VbsError(_js)
