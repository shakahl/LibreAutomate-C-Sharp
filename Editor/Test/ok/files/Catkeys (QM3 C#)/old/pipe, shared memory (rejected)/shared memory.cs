PF
__SharedMemory m.Create("yyyooo" 1024*1024)
PN
long* p=m.mem; p[0]=7
PN;PO
out m.mem

mac "sub.Thread"
1


#sub Thread
PF
__SharedMemory m.Open("yyyooo")
PN
long* p=m.mem; long k=p[0]
PN;PO
out m.mem
out k
