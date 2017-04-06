 /
function# $folders

 Finds in which of specified QM folders is currently open in editor QM item.
 Returns 1-based index of string in the folders list.
 Returns 0 if current item is not in one of specified folders.

 folders - multiline list of QM folder names.

 REMARKS
 Unavailable in exe.

 EXAMPLE
 int i=SelCurrentQmFolder("Folder A[]Folder B")
 out i ;;1 if current open macro is in folder "Folder A", 2 if in "Folder B", else 0


 get ancestor folder names
ARRAY(str) a
QMITEM q
qmitem "" 0 q 16
rep
	if(!q.folderid) break
	qmitem q.folderid 0 q 17
	a[]=q.name
 out a

str s; int i R
foreach s folders
	R+1
	for(i 0 a.len) if(s~a[i]) ret R
