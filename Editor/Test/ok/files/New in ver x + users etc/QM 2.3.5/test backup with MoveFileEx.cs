str s.all(1000000 2 'A')
str f.expandpath("$my qm$\test\test.txt")
str t.expandpath("$my qm$\test\test.txt.sav")
str b.expandpath("$my qm$\test\backup.txt")
 str b.expandpath("$temp$\backup.txt")
 str b.expandpath("G:\backup.txt")
Q &q

 s.setfile(t)
__HFile h.Create(t CREATE_ALWAYS GENERIC_WRITE 0)
WriteFile(h s s.len &_i 0)
FlushFileBuffers(h)
h.Close
 SetFileAttributes(f FILE_ATTRIBUTE_READONLY)
 SetFileAttributes(f FILE_ATTRIBUTE_NORMAL)

Q &qq
 CopyFileW(@f @b 0)
out MoveFileExW(@f @b MOVEFILE_REPLACE_EXISTING|MOVEFILE_COPY_ALLOWED)
int R=MoveFileExW(@t @f MOVEFILE_REPLACE_EXISTING)
if(!R) out _s.dllerror
Q &qqq
outq
out R
