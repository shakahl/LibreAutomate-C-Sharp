out
ICsv c=CreateCsv(1)

str s1 s2
s1=
 aaa,bbb,ccc
 dd, ee, ff
 g,  h , i
 j, k,l

c.FromString(s1)

ICsv c2
 c2=c.Copy
c2=c.Copy(1 0 1 2)

c2.ToString(s2)
out s2




 	ICsv'Copy([colFrom] [colCount] [rowFrom] [rowCount])
