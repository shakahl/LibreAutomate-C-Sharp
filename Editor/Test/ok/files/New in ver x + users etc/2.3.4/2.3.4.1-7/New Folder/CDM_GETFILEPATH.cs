 out GetCurDir

int w=win("Save As" "#32770")

 __ProcessMemory m.Alloc(w 1000)
 int n=SendMessageW(w CDM_GETFILEPATH 500 m.address)
int n=SendMessageW(w CDM_GETFILEPATH 500 _s.all(1000))
out n

 does not work on Windows 7
