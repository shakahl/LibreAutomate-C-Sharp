 Uses context menu. Does not work if fast.

 Sorts items in current QM folder by name.
 Current folder means where currently open/active macro is.
 Does not sort items in subfolders.

out
PF

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

PN
 move items: cut each item starting from the last sorted item, and paste as the first child of the parent folder
spe 100
int htv=id(2202 _hwndqm)
int htviParent=SendMessage(htv TVM_GETNEXTITEM TVGN_PARENT b[0].htvi)
act htv
 deb
for i b.len-1 -1 -1
	int htviFirst=SendMessage(htv TVM_GETNEXTITEM TVGN_CHILD htviParent)
	if(htviFirst=b[i].htvi) continue
	sub.RightClickItem htv b[i].htvi
	key mh
	sub.RightClickItem htv htviFirst
	key mm
PN;PO

 defocus treeview to hide checkboxes
act GetQmCodeEditor


#sub Callback_ARRAY_sort
function# param __SORTQMFOLDER&a __SORTQMFOLDER&b
ret StrCompareEx(a.name b.name 4)


#sub RightClickItem
function htv htvi
SendMessage htv TVM_ENSUREVISIBLE 0 htvi
RECT r.left=htvi; SendMessage htv TVM_GETITEMRECT 0 &r
 MapWindowPoints htv 0 +&r 2; OnScreenRect 0 r; 0.5
rig r.right+r.left/2 r.bottom+r.top/2 htv 1


#sub Close_MsgBox_MoveIntoFolder
rep
	int w=win("Quick Macros" "#32770" "" 0x1 "cClass=Static[]cText=Move into folder?")
	if(w) but 7 w; err ;;No
	0.01


#sub EndMifThread
function hThread
EndThread "" hThread
