function! $csvString $sep [flags] ;;flags: 1 no first column

 Populates grid control from a CSV string.
 The control must already have column headers.
 Returns: 1 success, 0 failed.

 csvString - the string. If "", just deletes all rows (if flag 1, clears all cells except first column).
 sep - separator character used in csvString. For example ",". If "", uses character defined in Control Panel.
 flags:
    1 (2.4.0) - don't delete rows. Replace cells except in first column.


ICsv c=CreateCsv
c.Separator=sep
c.FromString(csvString)
c.ToQmGrid(h flags&1<<1)
ret 1
err+
