 /
function $code [str&sOut] [$scriptArguments] [$wshOptions]

 Executes VBScript or other code in WSH (Windows Script Host).

 code - the script. By default it is interpreted as VBScript, but wshOptions allows you to use other languages supported by WSH.
   QM 2.3.5. Can be macro, like "macro:MacroName". Gets whole text, or text that follows #ret line. Ignores first line if it looks like macro options ( /...).
   QM 2.3.5. If "", gets caller's text that follows #ret line.
 sOut - variable that receives output (Echo, errors). If used, runs cscript.exe. If omitted or 0, runs wscript.exe, which shows output in message box.
 scriptArguments - string containing one or more values that the script can access through WScript.Arguments. See example.
 wshOptions - <google "WSH command line cscript wscript logo nologo">WSH options</google>. Can be used to specify language, eg "//E:JScript".

 REMARKS
 Similar to <help>VbsExec</help>, but executes the script in WSH, not in QM context.
 Saves code to a temporary file, runs WSH that executes it, then deletes the file.
 Can be used to run scripts containing WScript object, which cannot be used with VbsExec.

 See also: <Scripting help>
 Added in: QM 2.3.4.

 EXAMPLE
 WshExec "WScript.Echo WScript.Arguments.Item(1)" 0 "arg0 arg1"


#exe addtextof "<script>"
opt noerrorshere 1

if(!Scripting_GetCode(code _s)) _s=code
__TempFile tf.Init("vbs" "" "\WshExec" _s)

str cl=F"{wshOptions} ''{tf}'' {scriptArguments}"; cl.trim

if &sOut
	cl-"cscript.exe //NoLogo "
	RunConsole2 cl sOut "" 0x100
	if(sOut.end("[]")) sOut.fix(sOut.len-2)
else
	run "wscript.exe" cl "" "" 0x400

 info: the exit code not used for script errors.
