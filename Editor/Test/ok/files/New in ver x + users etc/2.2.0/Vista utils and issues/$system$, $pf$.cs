 dll kernel32 #Wow64DisableWow64FsRedirection *OldValue
 Wow64DisableWow64FsRedirection &_i

str s
out s.expandpath("$system$")
run s
out s.expandpath("$program files$")
run s
