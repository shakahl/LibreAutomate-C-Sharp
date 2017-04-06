 /test QmlToCompound
function $qmlFile $qmcFile

if(dir(qmcFile)) del- qmcFile

str s.getfile(qmlFile)
lpstr sep="[][0][0]"

 header
int k j=findb(s sep 4)
if(!s.beg("//QM v2.") or j<0) goto ge

IStorage x
IStream is
 if(StgCreateDocfile(@_s.expandpath(qmcFile) STGM_CREATE|STGM_WRITE|STGM_SHARE_EXCLUSIVE 0 &x)) end "failed" 1 ;;9.523 MB
 if(StgCreateDocfile(@_s.expandpath(qmcFile) STGM_CREATE|STGM_WRITE|STGM_SHARE_EXCLUSIVE|STGM_TRANSACTED 0 &x)) end "failed" 1 ;;looong, out of memory
 if(StgCreateDocfile(@_s.expandpath(qmcFile) STGM_CREATE|STGM_READWRITE|STGM_SHARE_EXCLUSIVE|STGM_SIMPLE 0 &x)) end "failed" 1 ;;loooong, Commit fails, 48 MB
if(StgCreateStorageEx(@_s.expandpath(qmcFile) STGM_CREATE|STGM_WRITE|STGM_SHARE_EXCLUSIVE STGFMT_DOCFILE 0 0 0 uuidof(IStorage) &x)) end "failed" 1 ;;9.523 MB


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
	
	is=0
	x.CreateStream(@_s.from(i) STGM_CREATE|STGM_WRITE|STGM_SHARE_EXCLUSIVE 0 0 &is)
	 x.CreateStream(@_s.from(i) STGM_READWRITE|STGM_SHARE_EXCLUSIVE 0 0 &is)
	is.Write(name j-j0 0)

if(x.Commit(0)) end "failed" 1

 err+ end _error
ret
 ge
end "bad file format"
