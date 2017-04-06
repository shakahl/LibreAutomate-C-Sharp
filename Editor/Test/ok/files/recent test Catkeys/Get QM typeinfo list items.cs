out
act GetQmCodeEditor
key CE .
Acc a1.Find(_hwndqm "LIST" "" "class=SysListView32[]id=2205" 0x1004)
ARRAY(Acc) a; a1.GetChildObjects(a 0 "LISTITEM" "" "" 16)
int i
for(i 0 a.len) out a[i].Name
key B


#ret
dialog
_s