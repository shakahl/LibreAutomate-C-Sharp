 /
function $folder mask func param

 Enumerates items in a QM folder. For each item, calls user-defined function.
 Error if the folder does not exist.

 folder - folder name or path. Use "" to include all macros. Also can be QM item id (see qmitem)(QM 2.3.0).
 mask - same as in <help>qmitem</help>.
 func - address of user-defined function that is called for each item. The function must begin with:
  function# iid QMITEM&q level param
  Parameters:
    iid - QM item id
    q - variable that contains more item's properties (<help "qmitem">see qmitem help</help>)
    level - 0 for direct children, 1 for children of direct subfolders, and so on
    param - same value that was passed to EnumQmFolder
  Should return 0. For folders, can return 1 to exclude the folder.

 See also: <GetQmItemsInFolder>. It is easier, uses array instead of callback function.


opt noerrorshere 1
if(!IsValidCallback(func 16)) end ERR_BADARG

ARRAY(QMITEMIDLEVEL) a
if(!GetQmItemsInFolder(folder &a)) end F"{ERR_MACRO} (folder)"

int i
for i 0 a.len
	QMITEMIDLEVEL r=a[i]
	QMITEM q; if(!qmitem(a[i].id 0 q mask)) end ERR_FAILED
	if 1=call(func a[i].id &q a[i].level param)
		for(i i+1 a.len) if(a[i].level<=r.level) break
		i-1
