function ARRAY(str)&a [$range] [flags] [ARRAY(word)&avt] ;;range examples: "" (all), "3:3" (row), "C:C" (column), "A1:C3" (range), "sel" (selection), ExcelRow(row) (row as variable).  flags: 1 date as number, 2 formula, 3 text

 Gets whole or part of worksheet into array.

 a - variable for data.
   This function creates array of 2 dimensions.
 range - part of worksheet from where to get data. Default: "" - the used range. <help>Excel range strings</help>.
 flags:
   0-3 - what Excel.Range function to use to get cells:
     0 (default) - Value. Gets cell text or internally stored numeric value. For date cells, gets date as string in format that is specified in Control Panel.
     1 - Value2. Same as 0, but gets date and currency cells as double numbers.
     2 - Formula. Same as 1, but from formula cells gets formula, not the calculated value.
     3 - Text. Gets cell text as displayed in Excel. Slower.
 avt - variable for cell value types.
   The function creates array of same dimensions as a.
   Can be these types: VT_EMPTY - empty cell, VT_BSTR - text, VT_R8 - number, VT_DATE - date, VT_CY - currency, VT_BOOL - true/false, VT_ERROR - error in formula cell.
   Don't use with flags 2 and 3.

 See also: <FE_ExcelSheet_Row>
 Added in: QM 2.3.3.
 Errors: Excel errors

 EXAMPLES
  /exe 1
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


WS
if(&avt and flags&3>1) end ERR_BADARG

int i j nr nc
Excel.Range r cell

GetRange(range r nr nc 1)

VARIANT v
sel flags&3
	case 0 v=r.Value
	case 1 v=r.Value2
	case 2 v=r.Formula ;;all BSTR
	 case 3 v=r.Text ;;error if not 1 cell
	case 3
	a.create(nc nr)
	for(i 0 nr) for(j 0 nc) cell=r.Item(i+1 j+1); a[j i]=cell.Text
	ret

if v.vt=VT_ARRAY|VT_VARIANT
	v.vt=0; ARRAY(VARIANT) av.psa=v.parray.psa
	nc=av.len; nr=av.len(1)
	a.create(nc nr)
	for(i 0 nr) for(j 0 nc) a[j i]=av[i+1 j+1]; err
	if(&avt) avt.create(nc nr); for(i 0 nr) for(j 0 nc) avt[j i]=av[i+1 j+1].vt
else ;;1 cell
	a.create(1 1)
	a[0 0]=v; err
	if(&avt) avt.create(1 1); avt[0 0]=v.vt

err+ E

 notes:
 Error if str=VARIANT(VT_ERROR).
