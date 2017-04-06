out
str s="one[]two[]three[]four"
ARRAY(str) a=s
IStringMap m=CreateStringMap
m.AddList(s)
IXml x=CreateXml
x.FromString("<x><i n=''one'' v1=''ooo'' /><i n=''two'' v1=''ttt'' /><i n=''three'' v1=''hhh'' /><i n=''four'' v1=''fff'' /></x>")
ICsv c=CreateCsv(1)
c.FromString(s)

int f1 f2 f3 f4
Q &q
int i
rep(1000) for(i 0 a.len) if(!StrCompare(a[i] "four")) f1=1; break
Q &qq
rep(1000) if(m.Get("four")) f2=1
Q &qqq
rep(1000) if(x.Path("x/i[@n='four']")) f3=1
Q &qqqq
rep(1000) for(i 0 c.RowCount) if(!StrCompare(c.Cell(i 0) "four")) f4=1; break
Q &qqqqq
outq

out "%i %i %i %i" f1 f2 f3 f4
