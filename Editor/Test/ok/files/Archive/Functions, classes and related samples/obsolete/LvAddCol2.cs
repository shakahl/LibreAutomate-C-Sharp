 /
function# hlv index $txt width

 Adds column to SysListView32 control that has LVS_REPORT style (1).
 Returns new column index.
 Index of first colunm is 0.
 Use -1 to add to the end.
 To create control with LVS_REPORT style, specify 0x54000001 style in dialog definition.


LVCOLUMNW col.mask=LVCF_WIDTH|LVCF_TEXT
col.pszText=@txt
col.cx=width
ret SendMessage(hlv LVM_INSERTCOLUMNW iif(index>=0 index 0xffff) &col)
