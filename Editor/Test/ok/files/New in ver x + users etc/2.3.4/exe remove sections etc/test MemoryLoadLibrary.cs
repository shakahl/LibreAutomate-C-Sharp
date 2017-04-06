dll "qm.exe"
	#MemoryLoadLibrary_ !*m
	#MemoryGetProcAddress_ hm $name
	MemoryFreeLibrary_ hm

str+ g_qmgrid.getfile("$qm$\qmgrid.dll")

int hm=MemoryLoadLibrary_(g_qmgrid)
outx hm
if(!hm) ret

WNDCLASS w
out GetClassInfo(0 "QM_grid" &w)
out w.lpszClassName

 MemoryFreeLibrary_ hm
