function! str&csvString $sep [flags] ;;flags: 1 no first column, 2 no empty rows, 4 selected/checked, 8 remove <...>

 Gets control content in CSV format.
 Returns: 1 success, 0 failed.

 csvString - variable that receives the CSV string.
 sep - separator character. For example ",". If "", uses character defined in Control Panel.
 flags - GRID.QG_GET_ constants. In QM 2.4.2 also added GRID.QG_GETEX_ constants.


ICsv c=CreateCsv
c.Separator=sep
c.FromQmGrid(h flags)
c.ToString(csvString)
ret 1
err+ csvString=""
