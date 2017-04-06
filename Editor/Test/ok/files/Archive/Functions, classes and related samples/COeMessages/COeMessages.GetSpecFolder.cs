function sf OEFOLDER&folder ;;sf: 0 inbox, 1 outbox, 2 sent, 3 deleted, 4 draft

 Gets special folder.

 folder - receives folder info. You can pass it to GetMessages or use dwFolderId with GetFolders. parentId will be 0.


ref MSOEAPI
if(!ns) end ES_INIT
IStoreFolder f
ns.OpenSpecialFolder(sf 0 &f); err end _error
FOLDERPROPS fp.cbSize=sizeof(fp)
f.GetFolderProps(0 &fp)

folder.cMessage=fp.cMessage
folder.cSubFolders=fp.cSubFolders
folder.cUnread=fp.cUnread
folder.folderId=fp.dwFolderId
lpstr name=&fp.szName; folder.name=name
folder.parentId=0
folder.sfType=fp.sfType
