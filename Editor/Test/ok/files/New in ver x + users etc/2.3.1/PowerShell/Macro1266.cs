out
 PowerShell "-ExecutionPolicy Unrestricted -Command {get-service | get-member}"
 PowerShell "-Command {get-service | get-member}"
 run "powershell.exe" "-noexit get-childitem c:\scripts"
 PowerShell "get-childitem c:\scripts"
str ps1Path="$documents$\Untitled1.ps1"
str cl.format("%s" _s.expandpath(ps1Path))
 str cl.format("-noexit &'%s'" _s.expandpath(ps1Path))
 str cl.format("&'%s'" _s.expandpath(ps1Path))
 run "powershell.exe" cl
 RunConsole "powershell.exe" cl
 RunConsole2 _s.from("powershell.exe " cl) 0 0 0

 RunConsole "powershell.exe" "-command get-process"

out RunConsole2("powershell.exe -command get-process")
 out RunConsole2("ipconfig /all")
 _s="gggggggggg"
 out RunConsole2("powershell.exe -command get-date" _s); out _s
 out RunConsole2("powershell.exe -help")
 out RunConsole2("cmd.exe /?")
 out RunConsole2("Q:\Projects\console\console\Release\console.exe")
