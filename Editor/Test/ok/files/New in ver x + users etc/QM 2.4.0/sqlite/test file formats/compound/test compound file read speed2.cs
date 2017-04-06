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
int i
for i 0 10000
	is=0
	x.OpenStream(@_s.from(i) 0 STGM_SHARE_EXCLUSIVE 0 &is)
	is.ToStr(_s is.GetSize)

PN
is=0
x=0
PN
PO

 speed: 1916  136683  1961  (500 fragments, buffered)
 speed: 99150  432367  1781  (1 fragment, not buffered)
 speed: 1955  135018  1945  (1 fragment, buffered)

 conslusion: much worse than sqlite. Loading 2-4 times slower, writing many times slower, sometimes out-of-memory error, 3 times more fragmentation, bigger file, not so easy to use.
