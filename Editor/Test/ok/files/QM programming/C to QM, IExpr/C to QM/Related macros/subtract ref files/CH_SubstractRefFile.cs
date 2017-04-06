 /Subtract ref
function $refFile $refFile1 [$refFile2] [$refFile3]

 Removes all declarations from refFile that also are available in some of other reffiles.
 Backups the old file as "filename (x).txt".


ARRAY(str) a a1
IStringMap m=CreateStringMap(2)
int i j

CH_RefFileToArray refFile a
out "%i in %s" a.len refFile

lpstr* sp=&refFile
for i 1 getopt(nargs)
	CH_RefFileToArray sp[i] a1
	out "%i in %s" a1.len sp[i]
	for j 0 a1.len
		m.Add(a1[j])
 out m.Count

str s ss
int n
for i 0 a.len
	if(m.Get(a[i])) continue
	if(a[i].beg("dll")) ;;one file may be with 'dll', other with 'dll-'
		if(a[i][3]='-') ss.from("dll" a[i]+4); else ss.from("dll-" a[i]+3)
		if(m.Get(ss)) continue
	n+1
	s.addline(a[i])
out "Added: %i.  In other files: %i.  Diff: %i" n m.Count m.Count-n

 s.setfile("$temp$\substractref.txt")
 run "$qm$\qm.exe" "''$temp$\substractref.txt''"

str backup=refFile
UniqueFileName backup
cop refFile backup

s.setfile(refFile)
 run "$qm$\qm.exe" F"''{refFile}''"
