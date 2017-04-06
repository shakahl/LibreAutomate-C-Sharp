out
ICsv c=CreateCsv
c.Separator=";"
c.FromFile("$my qm$\test.csv")

c.Cell(0 0)="new"

lpstr s
s=c.Cell(0 0)
out s
s=c.Cell(1 2)
out s

out "-----"
str ss
c.ToString(ss)
out ss
