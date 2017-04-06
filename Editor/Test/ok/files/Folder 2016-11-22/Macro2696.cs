out
PF
__SharedMemory m
 m.Open("Catkeys_SM_0x10000")
 PN
m.Create("Catkeys_SM_0x10000" 0x10000)
PN;PO
rep 5
	mac "sub.Thread"
	0.1


#sub Thread
PF
 __SharedMemory m.Open("Catkeys_SM_0x10000")
__SharedMemory m.Create("Catkeys_SM_0x10000" 0x10000)
PN;PO
out m.mem
