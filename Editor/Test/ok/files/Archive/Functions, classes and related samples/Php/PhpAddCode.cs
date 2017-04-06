 /
function $code [flags] ;;flags: 0 text, 1 macro, 2 file, 4 skip first line in macro.

 Adds PHP code that later can be used with other Php... functions.

 See also: <VbsAddCode>, <Php scripting help>.

 EXAMPLE
 PhpAddCode "function Add($a, $b) { return $a+$b; }"
 int sum=PhpFunc("Add" 1 2)
 out sum


class __PhpBugFix; __PhpBugFix- __phpbf

MSScript.ScriptControl- _php
VbsInit2 _php "PHPScript" code flags _s; err end _error

_php.AddCode(code); err end _error
if(_php.Error.Number) end VbsError(_php)
