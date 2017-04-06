function# [rowFrom]

 Returns index of first selected row.
 Returns -1 if there are no selected rows.

 rowFrom - if used, gets next selected row after rowFrom. Default -1.


if(getopt(nargs)=0) rowFrom=-1

ret Send(LVM_GETNEXTITEM rowFrom LVNI_SELECTED)
