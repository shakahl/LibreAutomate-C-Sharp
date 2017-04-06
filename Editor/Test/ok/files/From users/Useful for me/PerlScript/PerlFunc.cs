 /
function'VARIANT $func [VARIANT'a1] [VARIANT'a2] [VARIANT'a3] [VARIANT'a4] [VARIANT'a5] [VARIANT'a6] [VARIANT'a7] [VARIANT'a8] [VARIANT'a9] [VARIANT'a10]

 Calls PerlScript function that has been added with PerlScriptAddCode.

 func - name of one of added functions.
 a1, a2, ... - func arguments.

 EXAMPLE
  Add PerlScript code from file test.pl, and call functions it contains:
 PerlScriptAddCode "$desktop$\test.pl" 2
 out PerlScriptFunc("Sub1")
 out PerlScriptFunc("Sub2" 1 2.5 "arg3")


MSScript.ScriptControl- _PerlScript._create
_PerlScript.Language="PerlScript"

int nargs=getopt(nargs)-1
ARRAY(VARIANT) a.create(nargs)
VARIANT* pa=&a1
for(_i 0 nargs) a[_i].attach(pa[_i])

VARIANT v=_PerlScript.Run(func &a); err end _error
if(_PerlScript.Error.Number) end PerlError(_PerlScript)
