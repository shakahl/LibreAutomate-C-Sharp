 /
function $oldName $newName [flags] ;;flags: 1 skip folders, 2 skip shared...

 Renames QM item.

 oldName - used to find the item. Can be name, path, GUID or +id, like with <help>qmitem</help>. Error if QM item not found.
 new name - new name. If not unique, adds a suffix, eg "Abc" -> "Abc2".
 flags - <help>qmitem</help> flags. Used when oldName is name or path.

 REMARKS
 May stop working in some new QM version.
 Does nothing if the QM item is in a read-only folder, or is a read-only folder, or bad id.


int iid
if(oldName<0x10000) iid=oldName; else iid=qmitem(oldName flags)
if(!iid) end "QM item not found"

int c=id(2202 _hwndqm) ;;QM items tree view
NMTVDISPINFOW k.hdr.hwndFrom=c; k.hdr.idFrom=2202; k.hdr.code=TVN_ENDLABELEDITW
k.item.lParam=iid; k.item.pszText=@newName
SendMessageW(GetParent(c) WM_NOTIFY 2202 &k)
