 /
function# $filePath [$args] [$cl] [str&output]

 Executes PowerShell script file.
 Waits and returns powershell.exe's return value.
 Error if powershell.exe does not exist. PowerShell is available on Windows 7/2008, and can be downloaded for some older Windows versions.

 filePath - script file (.ps1). The function uses -File to pass it to powershell.exe.
 args - script arguments.
 cl - additional powershell.exe command line parameters.
 output - variable that receives output text. If omitted or 0, displays the text in QM output.

 See also: <PsCmd>

 EXAMPLES
 PsFile "$documents$\my script.ps1"
 PsFile "$documents$\my script.ps1" "" "-WindowStyle Maximized"


str s.format("%s -File ''%s'' %s" cl _s.expandpath(filePath) args)

ret PsRunCL(s output)
err end _error
