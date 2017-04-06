 /
function $code [flags] ;;flags: 2 file

 Adds VBScript code that later can be used with other Vbs... functions.

 code, flags - see <help>VbsExec</help>.

 REMARKS
 Code should contain functions and/or module-level declarations. Should not contain executable statements outside functions.

 See also: <VbsFunc>, <Scripting help>

 EXAMPLE
 lpstr vbs=
  Function Func(ByVal a)
  MsgBox a, , "VbScript Message"
  Func="From Func"
  End Function
 VbsAddCode vbs
 str s1="From QM"
 str s2=VbsFunc("Func" s1)
 mes s2


#exe addtextof "<script>"

MSScript.ScriptControl- _vbs
int iid=VbsInit(_vbs "VBScript" code flags _s); err end _error

_vbs.AddCode(code)
err end VbsError(_vbs iid)
