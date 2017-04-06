str s.all(7000000 2 'A')
str f.expandpath("$my qm$\test\test.txt")
str t.expandpath("$my qm$\test\test.txt.sav")
str b.expandpath("$my qm$\test\backup.txt")
 str b.expandpath("$my qm$\test\backup\backup.txt")
 str b.expandpath("$temp$\backup.txt")
 str b.expandpath("G:\backup.txt")
Q &q

 s.setfile(t)
__HFile h.Create(t CREATE_ALWAYS GENERIC_WRITE 0)
 WriteFile(h s s.len &_i 0)
int i n=10000; for(i 0 s.len n) WriteFile(h s+i n &_i 0)
FlushFileBuffers(h)
h.Close
 SetFileAttributes(f FILE_ATTRIBUTE_READONLY)
 SetFileAttributes(f FILE_ATTRIBUTE_NORMAL)

Q &qq
 int R=ReplaceFileW(@f @t 0 0 0 0)
int R=ReplaceFileW(@f @t @b REPLACEFILE_IGNORE_MERGE_ERRORS 0 0)
 int R=ReplaceFileW(@f @t @b REPLACEFILE_WRITE_THROUGH 0 0)
if(!R) out "%i %s" GetLastError _s.dllerror
Q &qqq
outq
out R
