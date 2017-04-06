str sf st
sf="I:\large.qml"
st="q:\my qm\test\test.qml"
DeleteFileCache sf; err
DeleteFileCache st; err
 del- st; err

iff st ;;erase
	__HFile f.Create(st OPEN_EXISTING)
	_s.all(8000000 2 0)
	if(!WriteFile(f _s _s.len &_i 0)) end "failed"
	FlushFileBuffers f
	f.Close
	out "erased"
	 ret

PF
if(!CopyFile(sf st 0)) out _s.dllerror
PN;PO

 testing: run this macro, and eject flash disk I: while in CopyFile.
