function! $name OEFOLDER&folder [parentFolderId]

 Finds folder.
 Returns 1 if found, 0 if not.

 name - folder name. Finds first matching folder. Case insensitive.
 folder - receives folder info.
 parentFolderId - must be 0 to search in all folders. Or can be dwFolderId of parent or ancestor folder's OEFOLDER.


ref MSOEAPI
if(!ns) end ES_INIT
ARRAY(OEFOLDER) af
GetFolders(af parentFolderId)
int i
for i 0 af.len
	OEFOLDER& f=af[i]
	if(f.name~name)
		folder=f
		ret 1
