 /CtoQM
function $shf [level] [issys]

 Calls H_file for all header files matching wildcard.
 Some files can be excluded, eg <c:\x\*.x!file1.h,file2.h,*file3.h>

str s
int i=findc(shf '!')
if(i<0) s=shf
else
	s.left(shf i)
	ARRAY(str) a
	tok shf+i+1 a -1 ","

Dir d
foreach(d s FE_Dir 0x4)
	str sPath=d.FileName(1)
	str sn=d.FileName
	 out sPath
	for(i 0 a.len) if(matchw(sn a[i] 1)) break
	if(i<a.len)
		 out "excluded: %s" sn
		continue
	H_file(sPath s level issys)
