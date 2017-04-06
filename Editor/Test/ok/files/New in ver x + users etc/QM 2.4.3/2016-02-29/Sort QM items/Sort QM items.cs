 Sorts items in current QM folder by name.
 Current folder means where currently open/active macro is.
 Does not sort items in subfolders.

out

 get QM item ids in current folder
int i
ARRAY(QMITEMIDLEVEL) a
QMITEM q; qmitem "" 0 q
if(q.folderid) GetQmItemsInFolder +q.folderid a 1; else GetQmItemsInFolder "" a 1
 for(i 0 a.len) out _s.getmacro(a[i].id 1)

 copy ids to array b, get names, and sort array b by name
type __SORTQMFOLDER iid htvi !isFolder ~name
ARRAY(__SORTQMFOLDER) b.create(a.len)
for(i 0 a.len) b[i].iid=a[i].id; qmitem a[i].id 0 q 1; b[i].htvi=q.htvi; b[i].isFolder=q.itype=5; b[i].name=q.name
b.sort(0 sub.Callback_ARRAY_sort)
 for(i 0 b.len) out b[i].name

 start thread that closes message box "Move into folder?"
atend sub.EndMifThread mac("sub.Close_MsgBox_MoveIntoFolder")

 move with Ctrl+X/V: cut each item starting from the last sorted item, and paste as the first child of the parent folder
 PerfFirst
BlockInput 1
spe 10
int htv=id(2202 _hwndqm)
int htviParent=SendMessage(htv TVM_GETNEXTITEM TVGN_PARENT b[0].htvi)
act htv
for i b.len-1 -1 -1
	int htviFirst=SendMessage(htv TVM_GETNEXTITEM TVGN_CHILD htviParent)
	if(htviFirst=b[i].htvi) continue
	SendMessage htv TVM_SELECTITEM TVGN_CARET|TVSI_NOSINGLEEXPAND b[i].htvi
	key Cx
	SendMessage htv TVM_SELECTITEM TVGN_CARET|TVSI_NOSINGLEEXPAND htviFirst
	key Cv
BlockInput 0
 PerfNext;PerfOut

 defocus treeview to hide checkboxes
act GetQmCodeEditor


#sub Callback_ARRAY_sort
function# param __SORTQMFOLDER&a __SORTQMFOLDER&b
ret StrCompareEx(a.name b.name 4)


#sub Close_MsgBox_MoveIntoFolder
rep
	int w=win("Quick Macros" "#32770" "" 0x1 "cClass=Static[]cText=Move into folder?")
	if(w) but 7 w; err ;;No
	0.01


#sub EndMifThread
function hThread
EndThread "" hThread
