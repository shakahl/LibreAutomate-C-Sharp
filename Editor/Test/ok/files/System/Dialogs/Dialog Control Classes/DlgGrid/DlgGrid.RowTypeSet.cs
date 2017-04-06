function row ctype ;;ctype: 0 edit, 1 combo, 2 check, 3 date, 7 read-only, 8 edit multiline, 9 combo sorted, 11 time, 16 (flag) with button, 23 read-only edit + button

 Sets control type for all cells in a row.
 The control must have "Can be set row control type" style (GRID.QG_SETROWTYPE, 4).

 row - 0-based row index.
 ctype - control type. See <help>DlgGrid.ColumnTypeSet</help>.

 Row control type also can be set when adding rows (FromCsv, RowAddSetMS, etc). See <help #IDP_QMGRID#details>grid row properties</help>.


Send(GRID.LVM_QG_SETCELLTYPE row ctype<<16)
