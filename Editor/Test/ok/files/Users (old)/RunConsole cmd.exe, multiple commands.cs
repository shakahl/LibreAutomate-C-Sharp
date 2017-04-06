str s=
 cd C:\
 dir
s.findreplace("[]" " && ")
RunConsole2 F"cmd.exe /C ''{s}''"

 system "''notepad.exe && notepad.exe''"
