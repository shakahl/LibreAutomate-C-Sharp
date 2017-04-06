out
str s=
 one, 5,    2/21/2012, 12:30
 two, 2,    5/10/2000, 2:10
 three, 31, 12/10/2000
 four, 1000

 s=
 one, 5,    2012.2.21, 12:30
 two, 2,    2000.5.10, 2:10
 three, 31, 2000.12.10
 four, 1000
 five, 5, www
 six, 6, aaa

ICsv c=CreateCsv(1)
c.FromString(s)

c.Sort(4|128 2)

str ss
c.ToString(ss)
out ss
