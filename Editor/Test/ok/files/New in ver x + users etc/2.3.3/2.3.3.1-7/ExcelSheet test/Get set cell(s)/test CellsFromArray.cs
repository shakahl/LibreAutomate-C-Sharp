ARRAY(str) a.create(2 5)
for(_i 0 a.len) a[0 _i]=_i; a[1 _i]=10*_i

ExcelSheet es.Init
 es.CellsFromArray(a F"A1:B{a.len}")
 es.SetCells2(a F"A1:B{a.len}")
 es.CellsFromArray(a "sel")
 es.CellsFromArray(a "A:B")
 es.SetCell("tt" 1 1)
 es.CellsFromArray(a "A1")
 es.CellsFromArray(a "C10")
 es.CellsFromArray(a "5:5")
 es.CellsFromArray(a)
