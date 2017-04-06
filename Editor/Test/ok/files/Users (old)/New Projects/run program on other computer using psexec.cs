 does not work (access denied)

 str cl="\\gintaras ''c:\windows\notepad.exe''"
str cl="\\gintaras -u G -p p ''c:\windows\notepad.exe''"
 run "$desktop$\psexec.exe" cl
RunConsole "$desktop$\psexec.exe" cl
