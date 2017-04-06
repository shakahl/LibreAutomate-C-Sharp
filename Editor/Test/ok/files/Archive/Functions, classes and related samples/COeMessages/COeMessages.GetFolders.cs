function ARRAY(OEFOLDER)&a [parentFolderId] [flags] ;;flags: must be 0

 Gets all folders, or all subfolders of a folder.

 a - receives folder info. OEFOLDER contains the same info as FOLDERPROPS, documented in MSDN library.
 parentFolderId - must be 0 to get all folders. Or can be dwFolderId of parent folder's OEFOLDER.


ref MSOEAPI
if(!ns) end ES_INIT
if(flags&0x10000=0) a=0

int h
lpstr name
FOLDERPROPS fp.cbSize=sizeof(fp)
ns.GetFirstSubFolder(parentFolderId &fp +&h)
if(!_hresult)
	rep
		OEFOLDER& f=a[]
		f.cMessage=fp.cMessage
		f.cSubFolders=fp.cSubFolders
		f.cUnread=fp.cUnread
		f.folderId=fp.dwFolderId
		name=&fp.szName; f.name=name
		f.parentId=parentFolderId
		f.sfType=fp.sfType
		
		if(fp.cSubFolders) GetFolders(a fp.dwFolderId flags|0x10000)
		ns.GetNextSubFolder(+h &fp)
		if(_hresult) break
	ns.GetSubFolderClose(+h)
