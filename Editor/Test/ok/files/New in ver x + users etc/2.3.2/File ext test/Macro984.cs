FileExtRegister "vig" "vigfile" "Vig File" "%SystemRoot%\System32\shell32.dll,20" "%SystemRoot%\System32\notepad.exe ''%1''"

 str s="test"
 s.setfile("$desktop$\test.vig")
 run "$desktop$\test.vig"
