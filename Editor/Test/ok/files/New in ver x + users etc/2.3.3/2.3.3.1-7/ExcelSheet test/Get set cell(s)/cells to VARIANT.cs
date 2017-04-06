out
ExcelSheet es.Init

int i
ARRAY(str) as
es.CellsToArray(as "A1:C3")
str* _as=&as[0 0]
for(i 0 9) out _as[i]

out "--------"

Excel.Range r=es._Range("A1:C3")
VARIANT v=r.Value
VARIANT* _av=&v.parray[1 1]
for(i 0 9) out _av[i]
