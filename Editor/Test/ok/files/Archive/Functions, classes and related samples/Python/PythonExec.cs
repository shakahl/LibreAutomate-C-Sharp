 /
function $code [flags] ;;flags: 2 file

 Executes Python code.

 code - Python code. Can be:
   String variable.
   File. Also use flag 2.
   Macro, like "macro:MacroName". Gets whole text, or text that follows #ret line. Ignores first line if it looks like macro options ( /...).
   If "", gets caller's text that follows #ret line.
 flags:
   2 - code is interpreted as file that contains script.

 REMARKS
 Python code can be one or more statements or/and functions.
 Functions and declarations also can be added with <help>PythonAddCode</help>.
 Code (functions, declarations) added by PythonAddCode and PythonExec is active in all functions of that thread (running macro). For example, you can call PythonAddCode in one QM function, and use it (eg call PythonEval) in another QM function. But when the thread ends, everything is cleared. 

 See also: <VbsExec>, <Python scripting help>.

 EXAMPLE
 PythonExec ""
 
 #ret
 import ctypes
 MessageBox = ctypes.windll.user32.MessageBoxA
 MessageBox(None, 'Hello', 'Window title', 0)


#exe addtextof "<script>"
#opt nowarnings 1

MSScript.ScriptControl- _python
int iid=VbsInit(_python "Python" code flags _s); err end _error

_python.ExecuteStatement(code); err end _error
if(_python.Error.Number) end VbsError(_python iid)
