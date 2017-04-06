ExcelSheet es.Init
ARRAY(str) a
int r c

  get and display column B
 es.CellsToArray(a "B:B")
 out "---- column B ----"
 for r 0 a.len ;;for each row
	 out a[0 r]
 
  get and display a row; use variable row index
 int row=2
 es.CellsToArray(a ExcelRow(row))
 out "[]---- row %i ----" row
 for c 0 a.len(1) ;;for each column
	 out a[c 0]
 
 get and display all used range
es.CellsToArray(a "")
out "[]---- all ----"
for r 0 a.len
	out "-- row %i --" r+1
	for c 0 a.len(1)
		out a[c r]
