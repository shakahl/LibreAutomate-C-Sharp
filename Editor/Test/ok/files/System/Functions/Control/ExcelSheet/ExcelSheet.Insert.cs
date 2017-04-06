function $range [shiftDirection] ;;range examples: "3:3" (row 3), "3:7" (5 rows), "C:C" (column C), ExcelRow(3).  shiftDirection: 0 auto, 1 to right, 2 down

 Inserts row(s), column(s) or other empty range.

 range - row, column or other range where to insert. <help>Excel range strings</help>.
 shiftDirection - where to shift other cells. Use when range is not full row(s)/column(s).

 Added in: QM 2.3.3.
 Errors: Excel errors

 EXAMPLE
  /exe 1
 ExcelSheet es.Init
 es.Insert("3:3") ;;insert row 3


WS

Excel.Range r=__Range(range)
VARIANT vsd=__VarOpt; sel(shiftDirection) case 1 vsd=Excel.xlShiftToRight; case 2 vsd=Excel.xlShiftDown
r.Insert(vsd)

err+ E

 tested: CopyOrigin does not work as expected.
