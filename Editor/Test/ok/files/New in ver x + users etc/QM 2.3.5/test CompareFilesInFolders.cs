out
ARRAY(COMPAREFIF) a

 CompareFilesInFolders a "$qm$" "$pf$\quick macros 2" "*.exe[]*.dll" "qmserv.exe" 0x100
 CompareFilesInFolders a "$qm$" "$pf$\quick macros 2" "*.exe" "qmserv.exe"
 CompareFilesInFolders a "$qm$" "$pf$\quick macros 2" "*" "qmserv.exe"
 CompareFilesInFolders a "$qm$" "$pf$\quick macros 2" "*.exe[]*.dll" "\qmnet*\*" 0x108
 CompareFilesInFolders a "$qm$" "$pf$\quick macros 2" "\qmnet*\*" "" 0x108
CompareFilesInFolders a "$qm$\htmlhelp" "Q:\app - new, VS2015, broken\HTMLHelp" "*.html" "" 0x008
 ret

int i
for i 0 a.len
	COMPAREFIF& x=a[i]
	
	str d1.getfile(x.f1) d2.getfile(x.f2); if(d1=d2) continue
	
	out F"0x{x.flags}  {x.f1%%-50s}  {x.f2}"
	
	 run F"$pf$\ExamDiff\ExamDiff.exe" F"''{x.f1}'' ''{x.f2}''"
