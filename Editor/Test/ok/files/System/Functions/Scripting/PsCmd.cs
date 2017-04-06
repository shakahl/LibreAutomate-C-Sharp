 /
function# $commands [$cl] [str&output]

 Executes PowerShell script stored in a variable or macro.
 Waits and returns powershell.exe's return value.
 Error if powershell.exe does not exist. PowerShell is available on Windows 7/2008/+, and can be downloaded for some older Windows versions.

 commands - one or more PowerShell commands. The function uses -EncodedCommand to pass it to powershell.exe.
   QM 2.3.5. Can be macro, like "macro:MacroName". Gets whole text, or text that follows #ret line. Ignores first line if it looks like macro options ( /...).
   QM 2.3.5. If "", gets caller's text that follows #ret line.
 cl - additional powershell.exe command line arguments.
 output - variable that receives output text. If omitted or 0, displays the text in QM output.

 See also: <PsFile>

 EXAMPLES
 PsCmd "get-date"

 PsCmd "get-date" "-OutputFormat XML"

 str code=
  get-process
 PsCmd code "" _s
 out _s

 PsCmd ""
 #ret
 get-date
 get-date;get-date


#exe addtextof "<script>"

Scripting_GetCode commands _s
str su.unicode(commands); su.encrypt(4)
str s.format("%s -EncodedCommand %s" cl su)

ret PsRunCL(s output)
err end _error
