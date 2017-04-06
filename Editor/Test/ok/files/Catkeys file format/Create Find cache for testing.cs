str f1="q:\test\ok"
str f2="q:\test\Find"
del- f2; err
mkdir f2
 int maxSize=2*1024*1024
 int maxSize=1024*1024
int maxSize=512*1024
 int maxSize=256*1024
str s
Dir d
int i
str sep="[][]_________________________________________________________[][]"
foreach(d "q:\test\ok\*" FE_Dir 2|4)
	str path=d.FullPath
	if(d.IsFolder) continue
	if(d.FileSize>=maxSize/2)
		cop path f2
		continue
	str t.getfile(path)
	if(s.len+t.len+sep.len>maxSize)
		s.setfile(F"{f2}\f{i}.txt")
		s.all
		i+1
	s+sep
	s+t

if s.len
	s.setfile(F"{f2}\f{i}.txt")
