out
str d.all(256 2 0)
d.set(1 0 33)
d[34]=1; d['<']=1; d['>']=1

 outb d 256

str s="one.one ''two two'' <three + three>"
lpstr s1 s2 s3
int n=tok(s &s1 3 d 1|4|64|256)
out n
out s1
out s2
out s3
