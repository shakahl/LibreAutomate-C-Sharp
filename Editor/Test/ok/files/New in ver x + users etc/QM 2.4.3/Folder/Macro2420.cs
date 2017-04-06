out
PF
foreach(0.5 1 FE_Wait 1)
	 break
	 act "hhhhhhhhh"
	 err out _error.description;; break
	 err out _error.description; goto g1
	out "loop"
	 err out _error.description; goto g1
	if(GetFileOrFolderSize("c:\file.txt")>0) break
	 err+ out "l2"
	 out "l3"
	0.2
 err out "e"
 g1
out "yes"
PN;PO
