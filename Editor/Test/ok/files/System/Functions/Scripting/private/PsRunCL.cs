 /
function# $cl [str&output]

str ps.searchpath("powershell.exe"); if(!ps.len) end "PowerShell not installed"
str s.format("%s %s" ps cl)
ret RunConsole2(s output)
err end _error
