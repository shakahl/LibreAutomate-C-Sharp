 /
function hlv index $txt width

 Adds column to SysListView32 control that has LVS_REPORT style (1).
 To create control with this style, specify 0x54000001 style in dialog definition.
 Index of first colunm is 0. If index <0, adds to the end.
 If width <0, it is interpreted as -percentage.


LVCOLUMNW col.mask=LVCF_WIDTH|LVCF_TEXT
col.pszText=@txt
if(width<0) RECT r; GetClientRect hlv &r; width=-width*r.right/100
col.cx=width
if(index<0) index=0x7fffffff
SendMessage hlv LVM_INSERTCOLUMNW index &col
