out
ARRAY(COMPAREFIF) a
CompareFilesInFolders a "q:\app" "q:\app - history" "*.exe[]*.htm" "\qmcore\*[]\qmgrid\*" 0x4
 CompareFilesInFolders a "q:\app" "q:\app - history" "\qmcl\*" "*.htm" 0x4
out "--------"
int i
for i 0 a.len
	COMPAREFIF& x=a[i]
	out F"0x{x.flags}  {x.f1}  {x.f2}"
