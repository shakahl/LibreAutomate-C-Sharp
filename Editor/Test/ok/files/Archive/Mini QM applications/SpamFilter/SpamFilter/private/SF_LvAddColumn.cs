 /
function hlv index $txt width

LVCOLUMN col.mask=LVCF_WIDTH | LVCF_TEXT
col.pszText=txt
col.cx=width
SendMessage hlv LVM_INSERTCOLUMN index &col
