out
ICsv c=CreateCsv
c.Separator=";"
c.FromFile("$my qm$\test.csv")

c.RemoveRow(1)
c.RemoveRow(c.RowCount-1)

 c.Clear
 c.Separator=""
 lpstr s1("1") s2("2") s3("3") s4("4")
 c.AddRowLA(0 4 &s1)

str ss
c.ToString(ss)
out ss
