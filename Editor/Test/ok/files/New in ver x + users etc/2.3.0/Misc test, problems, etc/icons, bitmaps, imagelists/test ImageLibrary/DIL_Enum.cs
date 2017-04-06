 /
function $folderPattern ARRAY(str)&a flags ;;flags: 1 extract all, 2 get document icons too

str s ss
int i n
Dir d
foreach(d folderPattern FE_Dir)
	s=d.FileName(1)
	n=ExtractIconEx(s -1 0 0 1)
	if(!n)
		if(flags&2) a[]=s
		continue
	 out s
	if(flags&1=0) n=1
	for(i 0 n)
		if(i or s.endi(".dll") or s.endi(".icl") or s.endi(".ocx"))
			ss.from(s "," i)
		else ss=s
		a[]=ss
