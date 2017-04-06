out
ICsv x._create
x.FromString("one[]two")
out x.ColumnCount

 x.AddRow2(-1 "a" "b")
 x.AddRowMS(1 3 "a[0]b[0]c[0]")
 x.ReplaceRowMS(1 3 "a[0]b[0]c[0]")
 x.Cell(1 2)="added"
x.Cell(2 2)="added"

out x.ColumnCount
x.ToString(_s); out _s
