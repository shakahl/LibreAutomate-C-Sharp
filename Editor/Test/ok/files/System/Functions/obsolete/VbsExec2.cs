 /
function ~code ;;obsolete, use WshExec.

 Executes VBScript code. Uses temporary .vbs file.

 REMARKS
 Similar to VbsExec, but this function saves the code to a temporary .vbs file, which is executed by the WSH engine, that is, not in QM context.
 This function does not wait.
 Can be used to run scripts containing Wscript object, which cannot be used with VbsExec (<link>http://support.microsoft.com/kb/279164</link>).
 Often it is simple to make the code compatible with VbsExec. For example, Wscript.Echo can be replaced with MsgBox.


str sf="$my qm$\VbsExec2.vbs"
code.setfile(sf)
run sf
