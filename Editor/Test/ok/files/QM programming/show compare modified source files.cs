 Dir d
 foreach(d "$qm$\*.cpp" FE_Dir 0x4)
	 str sPath=d.FileName(1)
	 out sPath
	 str sd.getfile(d.FileName(1));; err ...
	 long si=d.FileSize

 str folder1="q:\app"
 str folder2="q:\app - ANY reject"
str folder1="q:\app\tcc\project"
str folder2="q:\app\tcc26\project"
 str fn="*.cpp"
 str fn="*.h"
str fn="*.c"
int flags;;=4 ;;+subfolders

out
ARRAY(str) a1 a2 b1 b2
GetFilesInFolder a1 folder1 fn flags
GetFilesInFolder a2 folder2 fn flags
a1.sort(2)
a2.sort(2)
 out a1.len
 out a2.len
sub.SeparateNewFiles(a1 a2 b1 folder1.len folder2.len)
sub.SeparateNewFiles(a2 a1 b2 folder2.len folder1.len)
int i
for i 0 a1.len
	str& f1=a1[i]
	str& f2=a2[i]
	str s1.getfile(f1) s2.getfile(f2)
	if(s1=s2) continue
	 out _s.getfilename(f2 1)
	_s=
	F
	 run "$pf$\ExamDiff\ExamDiff.exe" F"''{f1}'' ''{f2}''"
	out _s
	 run "$pf$\ExamDiff\ExamDiff.exe" F"''{f1}'' ''{f2}''"

if b2.len
	out "ADDED:"
	for(i 0 b2.len) out b2[i]
if b1.len
	out "DELETED:"
	for(i 0 b1.len) out b1[i]


#sub SeparateNewFiles
function ARRAY(str)&a ARRAY(str)&a2 ARRAY(str)&b pathLen1 pathLen2
int i j
for i a.len-1 -1 -1
	lpstr s=a[i]+pathLen1
	int found=0
	for(j 0 a2.len) if(!StrCompare(a2[j]+pathLen2 s 1)) found=1; break
	if(!found) b[]=s; a.remove(i)
