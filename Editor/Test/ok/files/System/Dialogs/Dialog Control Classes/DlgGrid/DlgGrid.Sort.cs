function flags [column] ;;flags: 0 simple, 1 insens, 2 ling, 3 ling/insens, 4 number/ling/insens, 128 date, 0x100 descending, 0x10000 toggle, 0x20000 just set arrow

 Sorts.

 flags:
   0-0x100 - same as with <help>ICsv.Sort</help>.
   0x10000 - toggle.
   0x20000 - set the sort direction arrow in the column header, but don't sort.
 column - same as with ICsv.Sort.


Send(GRID.LVM_QG_SORT flags column)
