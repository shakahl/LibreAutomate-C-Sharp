 run "C:\WINDOWS\SYSTEM32\notepad.exe" "" "" "" 3 "+Notepad"
 run "C:\Program Files\Microsoft Office\OFFICE11\WINWORD.EXE"; 60 P; err
 run "winword" "" "" "" 0x200
 run "C:\WINDOWS\system32\cmd.exe" "" "" "" 0x400
 run "winword" "" "" "" 0x800 "Word"
int h
 run "winword" "" "" "" 0x1000 "Word" h
run "winword" "" "" "" 0x2800 "Word" h
out h
