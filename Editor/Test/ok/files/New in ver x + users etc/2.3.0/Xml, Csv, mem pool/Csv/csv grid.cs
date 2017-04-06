out
ICsv c=CreateCsv

int hwnd=id(1580 win("Options" "#32770"))
c.FromQmGrid(hwnd)
 c.FromQmGrid(hwnd 1)

c.Cell(1 1)="changed"

c.ToQmGrid(hwnd)

str ss
c.ToString(ss)
out ss
