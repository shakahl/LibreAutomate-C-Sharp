out
__HProcess p
p.Open(win("Notepad") SYNCHRONIZE)
 out p
out wait(0 H p.handle)
