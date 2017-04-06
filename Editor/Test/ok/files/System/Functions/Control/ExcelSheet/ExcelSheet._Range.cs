function'Excel.Range $range ;;range examples: "A1" (cell), "3:3" (row), "C:D" (2 columns), "A1:C3" (range), ExcelRow(3) (row), ExcelRange(3 2) (C2), "sel" (selection), "" (used range)

 Gets Excel.Range object for specified cells.

 range - cell, row, column or other range. <help>Excel range strings</help>.

 REMARKS
 A Range object represents a range of cells.
 You can call its functions to set formatting etc to the cells.
 Reference: Click the function, press F1, click Google in MS. Some info also may be in status bar.

 Added in: QM 2.3.3.
 Errors: Excel errors

 EXAMPLES. See also code of <open>ExcelSheet.Format</open> and other ExcelSheet functions.
  /exe 1
 ExcelSheet es.Init
 Excel.Range r=es._Range("C1")
 out r.Row
 out r.Column
 out r.NumberFormat
 r.NumberFormat="@" ;;Text
 r.Interior.Color=0x00e0ff
 r.Font.Italic=-1
 r.BorderAround(@ Excel.xlMedium 7)


WS

ret __Range(range)

err+ end _error
