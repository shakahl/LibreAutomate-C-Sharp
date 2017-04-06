 /exe 1
 assume we have 3 sheets that we will need to work with. Connect to each sheet using 3 ExcelSheet variables:
ExcelSheet es1.Init("name of sheet 1")
ExcelSheet es2.Init("name of sheet 2")
ExcelSheet es3.Init("name of sheet 3")

 get number of rows in first sheet
int nr
nr=es1.NumRows
out nr ;;show in QM output

 get all cells in sheet1. Don't know whether we need it, but here is just an example
ARRAY(str) a
es1.CellsToArray(a)
 show all cells in QM output
int c r
for r 0 a.len
	out "-- row %i --" r+1
	for c 0 a.len(1)
		out a[c r]
