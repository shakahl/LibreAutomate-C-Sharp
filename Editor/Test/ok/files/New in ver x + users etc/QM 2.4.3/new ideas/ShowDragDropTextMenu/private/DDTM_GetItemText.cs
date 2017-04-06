 /
function$ ICsv&c i

lpstr r=iif(c.ColumnCount>1 c.Cell(i 1) 0)
if(empty(r)) r=c.Cell(i 0); rep() if(r[0]=9) r+1; else break
ret r
