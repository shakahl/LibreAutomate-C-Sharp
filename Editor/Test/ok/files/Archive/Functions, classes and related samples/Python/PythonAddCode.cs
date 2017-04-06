 /
function $code [flags] ;;flags: 2 file

 Adds Python code that later can be used with other PythonX functions.

 code - Python code. Can be:
   String variable.
   File. Also use flag 2.
   Macro, like "macro:MacroName". Gets whole text, or text that follows #ret line. Ignores first line if it looks like macro options ( /...).
   If "", gets caller's text that follows #ret line.
 flags:
   2 - code is interpreted as file that contains script.

 See also: <PythonExec>, <VbsAddCode>, <Python scripting help>.

 EXAMPLE
 PythonAddCode ""
 int v=PythonFunc("Add" 2 3)
 out v
 
 #ret
 def Add(a, b):
	 return a+b


#exe addtextof "<script>"
#opt nowarnings 1

MSScript.ScriptControl- _python
int iid=VbsInit(_python "Python" code flags _s); err end _error

_python.AddCode(code); err end _error
if(_python.Error.Number) end VbsError(_python iid)
