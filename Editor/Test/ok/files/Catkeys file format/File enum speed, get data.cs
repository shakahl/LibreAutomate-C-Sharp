PF
int n nn
Dir d

 out
str s
int limit=10*1024
 s="q:\app\*" ;;5024 files, 10 MB, speed 2444/233
limit=-1
s="q:\test\ok\*" ;;11693 files, 10.8 MB, Avast 2510/1100, no AV 1944/536, WD 30377/615, notebook 17s/882
 s="E:\test\ok\*" ;;(HDD) 11693 files, 10.8 MB, Avast 7190/905, no AV 6519/569, WD 35702/660
 s="G:\test\ok\*" ;;(Sandisk flash) 11693 files, no AV 9606(USB3 7703)/797

 s="q:\test\find\*" ;;7 files, max 2M, 11.5 MB, Avast 39/5.4
 s="q:\test\find\*" ;;12 files, max 1M, 11.5 MB, Avast 34/4.4
 s="q:\test\find\*" ;;23 files, max 512K, 11.5 MB, Avast 36/5.2
 s="q:\test\find\*" ;;42 files, max 256K, 11.5 MB, Avast 43/6.5

foreach(d s FE_Dir 2|4)
	str path=d.FullPath
	if(d.IsFolder) continue
	if(d.FileSize>1024*1024*5) continue
	 out path
	 str t.getfile(path)
	str t.getfile(path 0 limit); err out F"error: {path}"; continue
	nn+t.len
	n+1
PN;PO
out "%i %g" n nn/1024.0/1024.0
