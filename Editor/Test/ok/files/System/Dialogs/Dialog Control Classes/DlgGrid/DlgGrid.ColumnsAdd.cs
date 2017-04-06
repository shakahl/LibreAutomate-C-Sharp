function! $columnsCSV [flags] ;;columnsCSV: "label1,width,ctype,cflags[]label2,width,ctype,cflags[]...".   ctype: 0 edit, 1 combo, 2 check, 3 date, 7 read-only, 8 edit multiline, 9 combo sorted, 11 time, 16 (flag) with button, 23 read-only edit + button.   flags: 1 will be vertical scrollbar, 2 grid options in first row

 Adds columns. Sets column widths and default control types.
 Returns: 1 success, 0 failed.
 Fails if the grid contains rows. If contains columns, deletes.

 columnsCSV - CSV-formatted string where each row defines grid column.
    Row format: "Label,Width,Ctype,Cflags". Width, Ctype and Cflags are optional, so it can be "Label" or "Label,Width" or "Label,,Ctype".
    Width can be percent, like 25%.
    Ctype and Cflags are as with <help>DlgGrid.ColumnTypeSet</help>. Must be simple numbers, not constant names.
    If columnsCSV is "", just deletes all columns.
 flags:
    1 - when calculating width %, assume there will be vertical scrollbar.
    2 - first row contains grid options. The whole CSV is like in dialog definition. Read more in <help #IDP_QMGRID>Help</help>.


ret Send(GRID.LVM_QG_ADDCOLUMNS flags +columnsCSV)
