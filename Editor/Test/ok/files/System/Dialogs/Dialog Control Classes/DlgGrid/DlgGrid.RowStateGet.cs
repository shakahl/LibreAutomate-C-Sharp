function# row ;;returns flags: LVIS_FOCUSED (1), LVIS_SELECTED (2), LVIS_CUT (4)

 Returns row state flags, except for images.

 row - 0-based row index.

 EXAMPLE
 if(g.RowStateGet(i)&LVIS_SELECTED) ...


ret Send(LVM_GETITEMSTATE row 0xffff00ff)
