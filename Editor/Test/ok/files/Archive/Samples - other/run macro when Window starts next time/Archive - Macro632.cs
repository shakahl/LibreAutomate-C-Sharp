 set registry value that tells Windows run QM and macro x when Windows starts. macro x will delete the value.
rset "qmcl.exe M ''Macro x''" "QM - macro x" "Software\Microsoft\Windows\CurrentVersion\Run"
