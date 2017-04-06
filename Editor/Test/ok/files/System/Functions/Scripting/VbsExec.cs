 /
function $code [flags] ;;flags: 2 file

 Executes VBScript code.

 code - VBScript code. Can be:
   String variable.
   File. Also use flag 2.
   QM 2.3.5. Macro, like "macro:MacroName". Gets whole text, or text that follows #ret line. Ignores first line if it looks like macro options ( /...).
   QM 2.3.5. If "", gets caller's text that follows #ret line.
 flags:
   2 - code is interpreted as file that contains script.
   Flags 1 and 4 are obsolete. 1 - code is interpreted as name of QM item that contains script. 4 (with flag 1) - exclude first line.

 REMARKS
 VBScript code can be one or more statements or/and functions.
 Functions and declarations also can be added with <help>VbsAddCode</help>.
 Code (functions, declarations) added by VbsAddCode and VbsExec is active in all functions of that thread (running macro). For example, you can call VbsAddCode in one QM function, and use it (eg call VbsEval) in another QM function. But when the thread ends, everything is cleared. 
 Does not support WScript object (<link>http://support.microsoft.com/kb/279164</link>). To run code that contains it, use <help>WshExec</help>. Or replace WScript functions with VBScript functions: WScript.Echo to MsgBox, WScript.CreateObject to CreateObject, etc.

 See also: <Scripting help>

 EXAMPLES
 str code=
  msgbox 1
  msgbox 2
 VbsExec code

 VbsExec "file.vbs" 2

 VbsExec "macro:VbsMacro"

 VbsExec ""
 #ret
 function Func()
 msgbox 1
 end function
 'call function Func:
 Func


#exe addtextof "<script>"

MSScript.ScriptControl- _vbs
int iid=VbsInit(_vbs "VBScript" code flags _s); err end _error

_vbs.ExecuteStatement(code)
err end VbsError(_vbs iid)
