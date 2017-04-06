function $range [shiftDirection] ;;range examples: "3:3" (row 3), "3:7" (5 rows), "C:C" (column C).  shiftDirection: 0 auto, 1 to left, 2 up

 Deletes row(s), column(s) or other range.

 range - row, column or other range to delete. <help>Excel range strings</help>.
 shiftDirection - where to shift other cells. Use when range is not full row(s)/column(s).

 Added in: QM 2.3.3.
 Errors: Excel errors

 EXAMPLE
  /exe 1
 ExcelSheet es.Init
 es.Delete("3:3") ;;delete row 3


WS

Excel.Range r=__Range(range)

VARIANT vsd=__VarOpt; sel(shiftDirection) case 1 vsd=Excel.xlShiftToLeft; case 2 vsd=Excel.xlShiftUp

r.Delete(vsd)

err+ E
