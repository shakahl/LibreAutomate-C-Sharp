str sf.expandpath("$my qm$\test\safe.txt")
str sft.expandpath("$my qm$\test\safe.txt-tmp")
str sd.all(50000 2 'c')

PF
File f.Open(sft "w")
 SetFilePointer(f 1000000 0 0); SetEndOfFile(f); SetFilePointer(f 0 0 0)
rep(1000000/sd.len)
	 WriteFile(f sd sd.len &_i 0)
	f.Write(sd 1 sd.len)
 SetEndOfFile(f)
PN
 fflush f.m_file
 FlushFileBuffers f
 _flushall f.m_file
f.Close
PN

 if(!CopyFile(sft sf 0)) end "failed"
if(!MoveFileEx(sft sf MOVEFILE_REPLACE_EXISTING)) end "failed"
PN;PO

 out GetFileFragmentation(sft)
out GetFileFragmentation(sf)
