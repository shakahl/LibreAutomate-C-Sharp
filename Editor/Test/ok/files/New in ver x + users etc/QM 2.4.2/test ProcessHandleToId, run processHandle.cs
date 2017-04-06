__Handle hp=run("notepad.exe")
int pid=ProcessHandleToId(hp)
out pid
int w=wait(30 WC win("" "Notepad" "" 0 F"processHandle={hp}"))
outw w

 PF
 run "notepad.exe" "" "" "" 0 win("Notepad")
  run "notepad.exe" "" "" "" 0x1000 win("Notepad")
 PN;PO
