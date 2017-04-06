 /
function view2

spe -1
Acc a
a=acc("Audio" "OUTLINEITEM" "Media Library" "SysTreeView32" "" 0x1001)
a.Select(3)
key F2
int h=wait(5 "Edit View")
str s=iif(view2 "Artist/album view" "Simple media view")
a=acc(s "RADIOBUTTON" h "Button" "" 0x1001)
a.DoDefaultAction
key Y
