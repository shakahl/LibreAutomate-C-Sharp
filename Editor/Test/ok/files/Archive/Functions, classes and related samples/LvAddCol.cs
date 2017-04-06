 /
function# hlv index $txt width

 Adds column to SysListView32 control that has LVS_REPORT style (1).
 To create control with this style, specify 0x54000001 style in dialog definition.
 Index of first colunm is 0.
 Returns new column index if successful, or -1 if failed.


LVCOLUMNW col.mask=LVCF_WIDTH|LVCF_TEXT
col.pszText=@txt
col.cx=width
ret SendMessage(hlv LVM_INSERTCOLUMNW index &col)
