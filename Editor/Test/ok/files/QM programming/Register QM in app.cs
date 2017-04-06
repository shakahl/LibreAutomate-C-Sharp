if(!IsUserAdmin) mes- "must be admin" "" "x"

str s

s.expandpath("$qm$\qm.exe")
rset(s "" "Software\Microsoft\Windows\CurrentVersion\App Paths\qm.exe" HKEY_LOCAL_MACHINE)

s.expandpath("$qm$\qmcl.exe")
rset(s "" "Software\Microsoft\Windows\CurrentVersion\App Paths\qmcl.exe" HKEY_LOCAL_MACHINE)

s.expandpath("$qm$\qm.exe ''%1''")
rset(s "" "qmlfile\shell\open\command" 0x80000000)

s.expandpath("$qm$\qm.exe LI ''%1''")
rset(s "" "qmlfile\shell\import\command" 0x80000000)

s.expandpath("$qm$\qmmacro.exe ''%1''")
rset(s "" "qmmfile\shell\open\command" 0x80000000)

 info: VS registers qmshex in app when building.
RegisterComComponent "$qm$\qmshex32.dll"
if(_win64) RegisterComComponent "$qm$\qmshex64.dll"
