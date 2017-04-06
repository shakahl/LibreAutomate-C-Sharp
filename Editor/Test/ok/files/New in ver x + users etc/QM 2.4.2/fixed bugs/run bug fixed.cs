out
 __Handle h=run("notepad.exe")
__Handle h=run("notepad.exe" "" "" "" 0 "Notepad" _i)
outw h
wait 0 H h
out "closed"

 out run("notepad.exe" "" "" "" 0x400 "Notepad" _i)
