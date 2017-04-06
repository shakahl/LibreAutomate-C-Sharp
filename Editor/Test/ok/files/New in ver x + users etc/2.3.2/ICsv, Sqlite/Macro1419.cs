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
c.AddRowMS(-1 3 "n1[0]n2[0]n3")
c.InsertColumn(-1)
 c.RemoveRow(2)
 c.RemoveColumn(2)
c.RemoveRow(3)
c.RemoveColumn(3)

 c.ToFile("$temp$\qm.csv"); c.FromFile("$temp$\qm.csv")
 c.ToString(_s); c.FromString(_s)

 c.GetRowMS(1 _s); outb _s _s.len
 c.MoveRow(1 2)
c.ReplaceRowMS(0 2 "r1[0]r2[0]r3")
 c.ColumnCount=5; c.AddRowMS(-1); c.AddRowMS(-1)
 c.ColumnCount=2; c.RemoveRow(1)

 for(i 0 c.RowCount) c.Cell(i i)="newcell wwwwwwwwwwwwwwwwwwwwwww"

 c.RowDataSize=30

str so
c.ToString(so)
out so

for(i 0 c.RowCount) out c.Cell(i i)
for(i 0 c.RowCount) c.GetRowMS(i _s); outb _s _s.len 1
for(i 0 c.RowCount) outb c.RowData(i) c.RowDataSize
