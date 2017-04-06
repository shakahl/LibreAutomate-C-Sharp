 /
function'VARIANT $func [VARIANT'a1] [VARIANT'a2] [VARIANT'a3] [VARIANT'a4] [VARIANT'a5] [VARIANT'a6] [VARIANT'a7] [VARIANT'a8] [VARIANT'a9] [VARIANT'a10]

 Calls PHP user-defined function that has been added with PhpAddCode.
 Also can be used to call PHP intrinsic/extension functions.

 See also: <VbsFunc>, <Php scripting help>.

 EXAMPLES
 PhpAddCode "function Add($a, $b) { return $a+$b; }"
 int sum=PhpFunc("Add" 1 2)
 out sum
 
 PhpFunc("shell_exec" "notepad")


class __PhpBugFix; __PhpBugFix- __phpbf

MSScript.ScriptControl- _php
VbsInit2 _php "PHPScript"; err end _error

int i n=getopt(nargs)-1
ARRAY(VARIANT) a.create(n)
VARIANT* pa=&a1
for(i 0 n) a[i].attach(pa[i])

VARIANT v=_php.Run(func &a); err end _error
if(_php.Error.Number) end VbsError(_php)
ret v
