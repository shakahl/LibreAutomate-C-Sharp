 /test QmlToQmf
function $qmlFile $qmcFile

if(dir(qmcFile)) del- qmcFile

str s.getfile(qmlFile)
lpstr sep="[][0][0]"

 header
int k j=findb(s sep 4)
if(!s.beg("//QM v2.") or j<0) goto ge

__HFile x.Create(qmcFile CREATE_ALWAYS GENERIC_WRITE)

 if(!WriteFile(x s s.len &_i 0)) end "failed" 1
 ret

 items
lpstr name triggerEtc flagsEtc folder text
int i
for i 0 2000000000
	j+4; if(j=s.len) break
	int j0=j
	name=s+j
	triggerEtc=name+len(name)+1 ;;can contain not only trigger
	flagsEtc=triggerEtc+len(triggerEtc)+1 ;;flags[ date[ image]]
	folder=flagsEtc+len(flagsEtc)+1
	text=strstr(folder "[]"); if(!text) goto ge
	j=findb(s sep 4 text-s+2); if(j<0) goto ge
	 out j-j0
	
	if(!WriteFile(x name j-j0 &_i 0)) end "failed" 1

 err+ end _error
ret
 ge
end "bad file format"
