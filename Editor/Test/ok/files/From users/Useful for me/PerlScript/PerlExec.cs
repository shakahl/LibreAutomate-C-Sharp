 /
function $statement [flags] ;;flags: 0 text, 1 macro, 2 file, 4 skip first line.

 Executes one or more Perl statements.
 The code can include functions.
 Functions also can be added with PerlAddCode.
 See also: <PerlAddCode>.

 EXAMPLE
 PerlExec "print 'hello world'"


#opt nowarnings 1

MSScript.ScriptControl- _PerlScript._create

_PerlScript.Language = "PerlScript"


if(flags&1)
	str s.getmacro(statement)
	if(flags&4) s.getl(s 1 2)
	statement=s
else if(flags&2) statement=s.getfile(statement)

_PerlScript.ExecuteStatement(statement); err end _error
if(_PerlScript.Error.Number) end PerlError(_PerlScript)
