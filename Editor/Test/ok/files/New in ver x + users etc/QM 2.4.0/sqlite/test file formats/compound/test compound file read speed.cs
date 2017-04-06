 1
str qmcFile="$my qm$\test\ok.qmc"

out GetFileFragmentation(qmcFile)
DeleteFileCache qmcFile

PF
IStorage x
__Stream is
int hr=StgOpenStorageEx(@_s.expandpath(qmcFile) STGM_SHARE_EXCLUSIVE STGFMT_DOCFILE 0 0 0 uuidof(IStorage) &x)
if(hr) end _s.dllerror("" "" hr)
PN
foreach _s "items[]names[]texts"
	is=0
	x.OpenStream(@_s 0 STGM_SHARE_EXCLUSIVE 0 &is)
	is.ToStr(_s is.GetSize)
	 out _s.len
	PN
is=0
x=0
PN
PO

