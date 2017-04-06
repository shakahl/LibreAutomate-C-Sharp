out
ICsv c=CreateCsv(1)
 out c.Separator
 c.Separator=","
 out c.Separator

 c.ColumnCount=5

str s1 s2
s1=
 a0,b0,c0
 a1, b1, c1
 a2,  b2 , c2
 a3, b3,c3
 a4, b4,c4

c.FromString(s1)

 c.ColumnCount=5
 c.ColumnCount=1
 c.ColumnCount=-1
 c.ColumnCount=70000

 c.Cell(1 1)="repl"
 out c.Cell(1 1)

str c1("r1") c2("r22222222") c3("r3")
 c.AddRowSA(1 3 &c1)
 c.AddRowSA(1 1 &c1)
 c.AddRowSA(1)
 c.AddRowSA(-1 3 &c1)
 c.ReplaceRowSA(1 3 &c1)
 c.ReplaceRowSA(1 1 &c1)
 c.ReplaceRowSA(1)
 c.ReplaceRowSA(-1 3 &c1)

 c.InsertColumn(-1)
 c.RemoveColumn(2)
 c.RemoveColumn(1)
 c.RemoveColumn(0)
 c.InsertColumn(1)
 c.InsertColumn(1)
 out c.ColumnCount

 c.RemoveRow(0)
 c.RemoveRow(0)
 c.RemoveRow(0)
 out c.RowCount
 out c.ColumnCount

 lpstr ms=c.Cell(1 0)
 c.AddRowMS(3 c.ColumnCount ms)
 c.RemoveRow(1)

 c.MoveRow(1 4)

str sr
c.GetRowMS(1 sr)
outb sr sr.len 1

c.ToString(s2)
out s2
