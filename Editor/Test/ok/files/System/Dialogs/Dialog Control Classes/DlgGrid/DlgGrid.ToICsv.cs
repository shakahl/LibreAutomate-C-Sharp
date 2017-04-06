function! ICsv&c [flags] ;;flags: 1 no first column, 2 no empty rows, 4 selected/checked, 8 remove <...>

 Gets control content into a ICsv variable.
 Returns: 1 success, 0 failed.

 c - variable of <help>ICsv</help> type. Must not be 0.
 flags - GRID.QG_GET_ constants. In QM 2.4.2 also added GRID.QG_GETEX_ constants.


c.FromQmGrid(h flags); err ret
ret 1
