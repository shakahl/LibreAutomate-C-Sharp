 /
function hlv index $txt width

 Adds column to SysListView32 control that has LVS_REPORT style (1).
 To create control with this style, specify 0x54000001 style in dialog definition.
 Index of first colunm is 0.


LVCOLUMN col.mask=LVCF_WIDTH | LVCF_TEXT
col.pszText=txt
col.cx=width
SendMessage hlv LVM_INSERTCOLUMN index &col
