out
ICsv c=CreateCsv

 c.FromFile("$my qm$\test.csv")
 c.Clear

c.AddRowMS(-1 2)
c.AddRowMS(-1 2)
c.Cell(0 0)="aaa"
c.Cell(0 1)="''bbb''"
c.Cell(0 1)="b"
c.Cell(1 1)="cccccccccccccc"

str ss
c.ToString(ss)
out ss
