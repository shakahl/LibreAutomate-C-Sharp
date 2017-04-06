 /
function'VARIANT $expression

 Evaluates a Perl expression and returns the result. The expression can include
 functions and module-level variables that have been added with PerlAddCode.

 EXAMPLES
 out PerlEval("10*(2+5);")

 PerlAddCode "my $v;"
 PerlExec "$v = 8;"
 out PerlEval("$v*2;")


MSScript.ScriptControl- _PerlScript._create
_PerlScript.Language="PerlScript"
VARIANT v=_PerlScript.Eval(expression); err end _error
if(_PerlScript.Error.Number) end PerlError(_PerlScript)
ret v
