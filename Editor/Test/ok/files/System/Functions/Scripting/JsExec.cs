 /
function $code [flags] ;;flags: 2 file

 Executes JScript code.

 code - JScript code. Can be:
   String variable.
   File. Also use flag 2.
   QM 2.3.5. Macro, like "macro:MacroName". Gets whole text, or text that follows #ret line. Ignores first line if it looks like macro options ( /...).
   QM 2.3.5. If "", gets caller's text that follows #ret line.
 flags:
   2 - code is interpreted as file that contains script.
   Flags 1 and 4 are obsolete. 1 - code is interpreted as name of QM item that contains script. 4 (with flag 1) - exclude first line.

 REMARKS
 JScript code can be one or more statements or/and functions.
 Functions and declarations also can be added with <help>JsAddCode</help>.
 Code (functions, declarations) added by JsAddCode and JsExec is active in all functions of that thread (running macro). For example, you can call JsAddCode in one QM function, and use it (eg call JsEval) in another QM function. But when the thread ends, everything is cleared. 

 See also: <VbsExec> (examples), <Scripting help>


#exe addtextof "<script>"

MSScript.ScriptControl- _js
int iid=VbsInit(_js "JScript" code flags _s); err end _error

_js.ExecuteStatement(code)
err end VbsError(_js iid)
