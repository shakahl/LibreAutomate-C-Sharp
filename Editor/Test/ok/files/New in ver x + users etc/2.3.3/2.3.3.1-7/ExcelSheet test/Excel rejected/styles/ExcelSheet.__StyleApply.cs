function $range $style

 Applies a style (formatting) to cells.

 range - cell, row, column or other range. <help>Excel range strings</help>.
 style - style name.
   Can be list of styles (multiline). For example, one style defines number format, other colors...

 REMARKS
 To create a style from existing formatted cell: click the cell, click menu Format -> Style, type style name, uncheck some, Add.
 Styles are saved in current workbook. You can open other worbook and import (Merge) its styles into current workbook using the same dialog.
 Also you can create or import styles at run time, using Excel functions.

 Added in: QM 2.3.3.

 EXAMPLE
 es.StyleApply("3:3" "yellow") ;;row 3


Excel.Range r=_Range(range)

foreach _s style
	r.Style=_s

err+ E
