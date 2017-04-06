out
int i
out
ICsv c=CreateCsv(1)

c.RowDataSize=4
 out c.RowData(1)

str si=
 a0,b0,c0
 a1,b1,c1
 a2,b2,c2

c.FromString(si)

for(i 0 c.RowCount) memset c.RowData(i) i+1 c.RowDataSize

 c.AddRowMS(1 3 "i1[0]i2[0]i3")
 c.InsertColumn(1)
 c.AddRowMS(1 2 "in1[0]in2[0]in3" 1)

 c.ReplaceRowMS(0 1 "re1[0]re2[0]re3" 1)
 str s1("aaa") s2("bbb") s3("ccc")
 c.ReplaceRowLA(0 3 &s1)
 c.ReplaceRowSA(0 1 &s1 1)
 c.InsertColumn(1)
 c.AddRowSA(1 2 &s1 1)

 for(i 0 c.RowCount) c.Cell(i i)="newcell wwwwwwwwwwwwwwwwwwwwwww"

 c.RowDataSize=0

str so
c.ToString(so)
out so

for(i 0 c.RowCount) out c.Cell(i i)
for(i 0 c.RowCount) c.GetRowMS(i _s); outb _s _s.len 1
for(i 0 c.RowCount) outb c.RowData(i) c.RowDataSize
