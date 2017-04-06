function hgrid $table $column [flags] ;;flags: 1 descending, 2 case insensitive, 0x100 except first column, 0x200 remove empty rows

 Sorts QM_Grid control.
 Error if something fails.

 hgrid - QM_Grid control handle.
 table - associated table name. Does not modify it.
 column - column name or 1-based index. Also can be SQL "ORDER BY" expression. Also probably can be used comma-separated list of these, although not tested.


FromQmGrid(hgrid "temp_sort" flags>>8&3 table) ;;use temporary table, because if using original table, user-made changes in the grid would be lost, or would need to save
_s.format("SELECT * FROM '%s' ORDER BY %s %s%s" "temp_sort" column iif(flags&2 "COLLATE NOCASE " "") iif(flags&1 "DESC" "ASC"))
ToQmGrid(hgrid _s flags>>8&1)
TempTable("temp_sort") ;;drop

err+ end _error
