 Makes IStream interface easier to use.
 All functions throw error if failed.
 This class is hidden, but you can use it.

 Added in: QM 2.3.2.

 EXAMPLES

 Creates or opens file, and creates stream on it.
__Stream x
str f="$desktop$\test __Stream.txt"
int fl=0x00000002 ;;STGM_READWRITE
if(!FileExists(f)) fl|0x00001000 ;;STGM_CREATE
x.CreateOnFile(f fl)
 get/set properties
int k=x.GetSize
out k
x.SetPos(k)
out x.GetPos
 write
str s="line[]"
x.is.Write(s s.len 0)

 _________________________

 Creates stream in memory from string.
__Stream x2
str s1="test"
x2.CreateOnHglobal(s1 s1.len)
str s2
x2.ToStr(s2 x2.GetSize 1)
out s2

 _________________________

 Creates empty stream in memory.
__Stream x3
int hg=x3.CreateOnHglobal
x3.is.Write("abc" 3 0)
out x3.GetPos
out hg
