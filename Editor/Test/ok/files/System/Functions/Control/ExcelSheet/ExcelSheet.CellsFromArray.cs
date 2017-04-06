function ARRAY(str)&a [$range] ;;range examples: "A1" (write from A1), ExcelRange(2 5) (from B5), "sel" (selection)

 Populates part of worksheet with data from 2-dim array.

 a - variable containing data.
   Can be values and/or =formulas.
 range - where in the worksheet to write the data. Default: "" the used range. <help>Excel range strings</help>.
   Regardless of range size (1 cell or more), always writes whole array starting from the top-left cell of the range.
   Does not touch other cells if range if bigger than array.

 Added in: QM 2.3.3.
 Errors: Excel errors

 EXAMPLE
  /exe 1
 ARRAY(str) a.create(2 10) ;;2 columns, 10 rows
 for(_i 0 a.len) a[0 _i]=_i; a[1 _i]=10*_i
 
 ExcelSheet es.Init
 es.CellsFromArray(a "A1")


WS

int i j nr(a.len) nc(a.len(1))
Excel.Range r=__Range(range)
r=r.Resize(nr nc)

ARRAY(VARIANT) av.create(nr nc)
for i 0 nr
	for j 0 nc
		av[i j]=a[j i]
r.Value=av

err+ E
