 Deb
 str s; int n
 byte* b=ExeLoadDataResource(100 n)
 if(!b) ret
 s.fromn(b n)
 out s

str sf="$temp$\temp.txt"
if(!ExeResourceToFile(100 sf)) ret
run sf
