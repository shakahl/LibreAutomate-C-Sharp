function row column ctype ;;ctype: 0 edit, 1 combo, 2 check, 3 date, 7 read-only, 8 edit multiline, 9 combo sorted, 11 time, 16 (flag) with button, 23 read-only edit + button

 Sets control type for a cell.

 row - 0-based row index.
 column - 0-based column index.
 ctype - control type. See <help>DlgGrid.ColumnTypeSet</help>.

 Cell control type also can be set when adding rows (FromCsv, RowAddSetMS, etc). See <help #IDP_QMGRID#details>grid row properties</help>.


Send(GRID.LVM_QG_SETCELLTYPE row MakeInt(column ctype))
