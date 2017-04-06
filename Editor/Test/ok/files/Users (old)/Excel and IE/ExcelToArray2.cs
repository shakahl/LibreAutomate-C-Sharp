 /
function ARRAY(str)&a [$range] [VARIANT'sheet] ;;sheet: 0 (active sheet), or sheet name, or 1-based index;  range examples: "" (all), "sel" (selection), "A1:C3" (range), press F1 to see more

 Stores Excel sheet into two-dimensional array.
 The sheet must be open in Excel.
 If range is used and not "", gets only the specified cells.
   If sheet is omitted or 0, range can be "sel" to get selected cells.
   range examples: "sel" (selection), "A1:C3" (range), "A:C" (columns), "3:3" (row), "3:3, 5:5, 7:7" (noncontiguous rows), "A1" (cell), "Named" (named range)

 EXAMPLE
 ARRAY(str) a
 ExcelToArray2 a
 int r c
 for r 0 a.len
	 out "-----Row %i-----" r+1
	 for c 0 a.len(1)
		 out a[c r]


typelib Excel {00020813-0000-0000-C000-000000000046} 1.2
#opt dispatch 1 ;;call functions through IDispatch::Invoke (may not work otherwise)

Excel.Application xlApp._getactive; err act; act; xlApp._getactive
Excel.Worksheet ws
Excel.Range r ru
int isrange=len(range)

if((sheet.vt=VT_I4 and sheet.lVal) or (sheet.vt=VT_BSTR and sheet.bstrVal.len))
	ws=xlApp.Worksheets.Item(sheet)
else
	ws=xlApp.ActiveSheet
	if(isrange and !q_strnicmp(range, "sel", 3)) r=xlApp.Selection; goto g1
if(isrange) r=ws.Range(range)
else r=ws.UsedRange
 g1
int i j nr(r.Rows.Count) nc(r.Columns.Count)
 limit to used range
if(nr=65536) ru=ws.UsedRange; nr=ru.Row-1+ru.Rows.Count
if(nc=256) ru=ws.UsedRange; nc=ru.Column-1+ru.Columns.Count

a.create(nc nr)
for i 0 nr
	for j 0 nc
		ru=r.Item(i+1 j+1)
		a[j i]=ru.Value

err+ end _error
