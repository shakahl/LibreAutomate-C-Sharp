 /
function $folder mask func param
 EnumQmFolder
 Enumerates items in a QM folder. For each item, calls user-defined function.

 folder - folder name or path. Use "" to include all macros.
 mask - same as in qmitem (see QM Help).
 func - address of user-defined function that is called for each item.
 Function must be:
  function# iid QMITEM&q level param
  Arguments:
   iid - QM item id
   q - structure that contains more item's properties (see QM Help, qmitem topic)
   level - 0 for direct children, 1 for children of direct subfolders, and so on
   param - same value that was passed to EnumQmFolder
  Return value should be 0. For folders, can return 1 to exclude the folder.


if(IsBadCodePtr(func)) end ERR_BADARG
QMITEM q; int iid
if(len(folder)) iid=qmitem(folder 0 q); if(q.itype!5) end "folder not found"
EQ_Rec id(2202 _hwndqm) iid q func param mask 0