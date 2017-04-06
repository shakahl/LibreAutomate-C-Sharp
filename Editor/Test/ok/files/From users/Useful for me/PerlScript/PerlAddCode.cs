 /
function $code [flags] ;;flags: 0 text, 1 macro, 2 file, 4 skip first line.

 Adds PerlScript code that later can be used with PerlFunc and other Perl... functions.

 flags:
   1 - code is interpreted as name of QM item that contains PerlScript code.
   2 - code is interpreted as file (.pl, .pm, whatever) that contains PerlScript code.
   4 - exclude first line.

 The code should contain functions and module-level declarations.
 It should not contain executable statements outside functions.

 EXAMPLE
  Exchanging data between QM and PerlScript.
 lpstr PerlScript=
  Function Func(ByVal a)
  MsgBox a, , "PerlScript Message"
  Func="From Func"
  End Function
 PerlAddCode PerlScript
 
 str s1="From QM"
 str s2=PerlFunc("Sub" s1)
 mes s2


#opt nowarnings 1

MSScript.ScriptControl- _PerlScript._create
_PerlScript.Language="PerlScript"

if(flags&1)
	str s.getmacro(code)
	if(flags&4) s.getl(s 1 2)
	code=s
else if(flags&2) code=s.getfile(code)

_PerlScript.AddCode(code); err end _error
if(_PerlScript.Error.Number) end PerlError(_PerlScript)
