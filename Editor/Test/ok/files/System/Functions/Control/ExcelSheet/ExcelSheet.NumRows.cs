function# [int&nColumns]

 Gets number of used rows and columns, including empty rows/columns at the beginning.
 Returns number of rows.

 nColumns - variables that receive number of columns. Can be 0 if don't need.

 REMARKS
 The used range is the part of worksheet containing non-empty cells (data, formatting).

 See also: <ExcelSheet.GetRangeInfo>.
 Errors: Excel errors


WS(1)

Excel.Range r=ws.UsedRange
if(&nColumns) nColumns=r.Columns.Count+r.Column-1
ret r.Rows.Count+r.Row-1

err+ E

 speed:
 This is faster than SpecialCells(Excel.xlCellTypeLastCell),
 same speed as r=r.Item(r.Count),
 2 times slower than Address, but safer.
