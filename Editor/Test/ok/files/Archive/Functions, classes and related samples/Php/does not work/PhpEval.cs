 /
function'VARIANT $expression

 Evaluates PHP expression and returns result.

 NOTE: It seems that PHP active scripting engine does not support evaluating expressions. Or I don't know something.

 See also: <VbsEval>.


class __PhpBugFix; __PhpBugFix- __phpbf

MSScript.ScriptControl- _php
VbsInit2 _php "PHPScript"; err end _error

VARIANT v=_php.Eval(expression); err end _error
if(_php.Error.Number) end VbsError(_php)
ret v
