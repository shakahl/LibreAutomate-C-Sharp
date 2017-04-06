function! ICsv&c [flags] ;;flags: 1 no first column

 Populates grid control from a ICsv variable.
 The control must already have column headers.
 Returns: 1 success, 0 failed.

 c - variable of <help>ICsv</help> type. Must not be 0.
 flags:
    1 (2.4.0) - don't delete rows. Replace cells except in first column.


c.ToQmGrid(h flags&1<<1); err ret
ret 1
