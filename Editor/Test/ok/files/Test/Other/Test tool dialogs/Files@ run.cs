 run "C:\WINDOWS\SYSTEM32\accwiz.exe"
 run "C:\WINDOWS\SYSTEM32\notepad.exe" "" "" "" 0x800 "Notepad"
 run "control.exe" "main.cpl,Keyboard,1"
 run "$Desktop$\test.txt" "" "" "" 3
 run s
 run "C:\WINDOWS\SYSTEM32\notepad.exe" "" "" "$Desktop$"
 run "C:\WINDOWS\SYSTEM32\notepad.exe" p "" f; 60 P; err
 run "C:\WINDOWS\SYSTEM32\notepad.exe"; err ErrMsg(2)
 60 P; err
 run "notepad.exe" "" "" "" 0x400

 int ec
 ec=run("notepad" "" "" "" 0x400)
 out ec

 int h
 run "notepad" "" "" "" 0x800 "Notepad" h
 out h

 int h e
 e=run("notepad" "" "" "" 0xC00 "Notepad" h)

 run "notepad" "" "" "" 0x2100 win("Notepad" "" "" 0x6)
 run "notepad" "" "" "" 0x1800 "Notepad"
 run "notepadd" "" "" "" 0x100
 err
	 out "cha"
 out 1

 run "$Favorites$\"
