ILockBytes lb
IStorage sto
int grfMode=STGM_READWRITE|STGM_CREATE|STGM_SHARE_EXCLUSIVE
if(CreateILockBytesOnHGlobal(0 1 &lb)) ret
 out lb
int hr=StgCreateDocfileOnILockBytes(lb grfMode 0 &sto)
if(hr) out _s.dllerror("" "" hr); ret
 out sto
 sto.EnumElements
int i
for i 0 1
	IStream is
	sto.CreateStream(@F"test{i}" grfMode 0 0 &is)
	 out is
	rep(100) is.Write(L"data" 10 _i)
	 out _i
	is=0

STATSTG stat
lb.Stat(stat STATFLAG_NONAME)
long& size=+&stat.cbSize
out size
 2560

