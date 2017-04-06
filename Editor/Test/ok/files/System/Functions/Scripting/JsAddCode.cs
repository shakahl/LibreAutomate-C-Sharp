 /
function $code [flags] ;;flags: 2 file

 Adds JScript code that later can be used with other Js... functions.

 code, flags - see <help>VbsExec</help>.

 See also: <VbsAddCode>, <JsFunc>, <Scripting help>


#exe addtextof "<script>"

MSScript.ScriptControl- _js
int iid=VbsInit(_js "JScript" code flags _s); err end _error

_js.AddCode(code)
err end VbsError(_js iid)
