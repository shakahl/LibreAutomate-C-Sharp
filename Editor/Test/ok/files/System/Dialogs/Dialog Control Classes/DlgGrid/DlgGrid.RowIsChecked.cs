function! row

 Returns 1 if checked, 0 if not.

 row - 0-based row index.


ret Send(LVM_GETITEMSTATE row 0xF000)>>12&15=2
