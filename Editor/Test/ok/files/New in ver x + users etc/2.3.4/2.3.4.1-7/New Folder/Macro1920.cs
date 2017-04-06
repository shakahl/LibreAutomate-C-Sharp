out
str s s1 s2 f

s="aaa"
s.setfile("$desktop$\test.txt")

s="bbb"
 f.expandpath("$desktop$\test.txt:fs")
f.expandpath("$desktop$\test.txt:fs:$DATA")
s.setfile(f)

out s1.getfile("$desktop$\test.txt")
out s2.getfile(f)

out dir(f)
out _s.searchpath(f)

outx GetFileAttributes(f)

WIN32_FIND_DATA d
int h=FindFirstFile(f &d)
 int h=FindFirstFileEx(f 0 &d 0 0 0)
out h

if(h!-1) FindClose(h)
