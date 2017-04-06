function column ctype [cflags] ;;ctype: 0 edit, 1 combo, 2 check, 3 date, 7 read-only, 8 edit multiline, 9 combo sorted, 11 time, 16 (flag) with button, 23 read-only edit + button.  cflags: higher priority than row type

 Sets default control type for all cells in a column.
 It will be applied to cells when adding rows.
 When control type is not set, it is 0 (edit).

 column - 0-based column index.
 ctype - column control type. To also add button, add flag 16.
   There are constants, defined in reference file GRID:
      QG_EDIT (0), QG_COMBO (1), QG_CHECK (2), QG_NONE (7),
      QG_EDIT_MULTILINE (8), QG_COMBO|QG_COMBO_SORT (9),
      QG_BUTTONATRIGHT (16).

 You can also set control types when calling ColumnsAdd.


Send(GRID.LVM_QG_SETCOLUMNTYPE column MakeInt(ctype cflags))
