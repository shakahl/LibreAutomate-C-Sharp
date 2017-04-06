 /test QmlToCompound
function $qmlFile $qmcFile

if(dir(qmcFile)) del- qmcFile

str s.getfile(qmlFile)

IStorage x
if(StgCreateStorageEx(@_s.expandpath(qmcFile) STGM_CREATE|STGM_WRITE|STGM_SHARE_EXCLUSIVE STGFMT_DOCFILE 0 0 0 uuidof(IStorage) &x)) end "failed" 1 ;;9.523 MB

IStream items names texts
x.CreateStream(@"items" STGM_CREATE|STGM_WRITE|STGM_SHARE_EXCLUSIVE 0 0 &items)
x.CreateStream(@"names" STGM_CREATE|STGM_WRITE|STGM_SHARE_EXCLUSIVE 0 0 &names)
x.CreateStream(@"texts" STGM_CREATE|STGM_WRITE|STGM_SHARE_EXCLUSIVE 0 0 &texts)
int i n
n=s.len/10
items.Write(s n 0)
names.Write(s+n n 0)
n*2
texts.Write(s+n s.len-n 0)

if(x.Commit(0)) end "failed" 1

 err+ end _error
