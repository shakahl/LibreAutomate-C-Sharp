str sf.expandpath("$my qm$\test\safe.txt")
str sft.expandpath("$my qm$\test\safe.txt-tmp")
str sd.all(50000 2 'c')

PF
 __HFile f.Create(sft CREATE_ALWAYS GENERIC_WRITE 0 FILE_ATTRIBUTE_TEMPORARY)
__HFile f.Create(sft CREATE_ALWAYS GENERIC_WRITE)
 SetFilePointer(f 1000000 0 0); SetEndOfFile(f); SetFilePointer(f 0 0 0)
rep(1000000/sd.len)
 rep 1
	WriteFile(f sd sd.len &_i 0)
 SetEndOfFile(f)
 SetAttr sft FILE_ATTRIBUTE_ARCHIVE
PN
FlushFileBuffers f
f.Close
PN

 if(!CopyFile(sft sf 0)) end "failed"
if(!MoveFileEx(sft sf MOVEFILE_REPLACE_EXISTING)) end "failed"
PN;PO

 out GetFileFragmentation(sft)
out GetFileFragmentation(sf)
