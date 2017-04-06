 /
function $code [flags] ;;flags: 0 text, 1 macro, 2 file, 4 skip first line in macro.

 Executes PHP code.

 See also: <VbsExec>, <Php scripting help>.

 EXAMPLE
 str code=
  shell_exec('notepad');
  shell_exec('calc');
 PhpExec code


class __PhpBugFix; __PhpBugFix- __phpbf

MSScript.ScriptControl- _php
VbsInit2 _php "PHPScript" code flags _s; err end _error

_php.ExecuteStatement(code); err end _error
if(_php.Error.Number) end VbsError(_php)
