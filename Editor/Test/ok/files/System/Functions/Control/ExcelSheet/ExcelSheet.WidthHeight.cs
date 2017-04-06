function $range [^columnWidth] [^rowHeight] [flags] ;;columnWidth: num chars in Normal style, -1 std, -2 auto fit.  rowHeight: num of standard height, -1 std, -2 auto fit.  flags: 1 set standard width

 Sets column width or/and row height.

 range - any range. Sets with/height of all columns/rows of the range. <help>Excel range strings</help>.
 columnWidth - if nonzero, sets column width.
   The unit is width of character "0" in Normal style.
   If -1, use standard
   If -2, auto fit existing text.
 rowHeight - if nonzero, sets row height.
   The unit is number of standard row heights.
   If -1, use standard.
   If -2, auto fit existing text.
 flags:
   1 - set standard column width. Does not use range and rowHeight.

 Added in: QM 2.3.3.
 Errors: Excel errors

 EXAMPLES
 es.WidthHeight("C:E" 20) ;;set width of columns C-E to the width of 20 characters of Normal style
 es.WidthHeight("5:5" 0 1.5) ;;set height of row 5 to 1.5 of standard height


WS

if(flags&1) ws.StandardWidth=columnWidth; ret

Excel.Range r=__Range(range)

if(columnWidth>0) r.ColumnWidth=columnWidth
else if(columnWidth=-1) r.UseStandardWidth=-1
else if(columnWidth=-2) r.EntireColumn.AutoFit

if(rowHeight>0) r.RowHeight=ws.StandardHeight*rowHeight
else if(rowHeight=-1) r.UseStandardHeight=-1
else if(rowHeight=-2) r.EntireRow.AutoFit

err+ E
