 /
function# str'sourceFolder str'destFolder [str'filesToCopy]

 Copies files from a virtual (or normal) folder to a normal (or virtual) folder.
 Returns the number of copied items (direct children of sourceFolder). Error if fails.

 sourceFolder - source folder pidl string.
   To get folder pidl string, Shift+drag and drop it to the QM code editor.
 destFolder - destination folder path.
   If folder does not exist, creates, unless destFolder does not contain \ character (eg it is a virtual folder).
 filesToCopy - filename. Use wildcard for multiple files.
   Applied only to direct child items.
   If omitted or empty, copies all files and subfolders.

 REMARKS
 Not for Windows XP.

 EXAMPLE
 str sourceFolder="$17$ 3A002E803ACCBFB42CDB4C42B0297FE99A87C641260001002500EFBE110000000A5AA837A573D0011C66AFAEC70DD1011C66AFAEC70DD1011400" ;;Desktop (example)
 str destFolder="Q:\Test\Sub"
 CopyFilesFromVirtualFolder sourceFolder destFolder "*.csv"


opt noerrorshere
if(_winver<0x600) end "not for Windows XP"

 get IShellItem of the source folder
ITEMIDLIST* pi=PidlFromStr(sourceFolder); if(!pi) end F"{ERR_FAILED}. Possibly invalid sourceFolder"
IShellItem siFolder1
int hr=WINAPIV.SHCreateItemFromIDList(pi IID_IShellItem &siFolder1); CoTaskMemFree pi
if(hr) end F"{ERR_FAILED}. Possibly invalid sourceFolder"

 get IShellItem of the destination folder
if(findc(destFolder '\')>=0 and !FileExists(destFolder 1)) mkdir destFolder
pi=PidlFromStr(destFolder); if(!pi) end F"{ERR_FAILED}. Possibly invalid destFolder"
IShellItem siFolder2
hr=WINAPIV.SHCreateItemFromIDList(pi IID_IShellItem &siFolder2); CoTaskMemFree pi
if(hr) end F"{ERR_FAILED}. Possibly invalid destFolder"

 get IEnumShellItems
IEnumShellItems en
siFolder1.BindToHandler(0 BHID_EnumItems IID_IEnumShellItems &en)

 create IFileOperation
WINAPIV.IFileOperation fo._create(CLSID_FileOperation)

 enumerate items
int R
rep ;;doesn't work: foreach siFolder1 en
	 get child item IShellItem
	IShellItem si=0; en.Next(1 &si &_i); if(!_i) si=0; break
	 copy only matching items
	if filesToCopy.len
		word* dn; si.GetDisplayName(SIGDN_PARENTRELATIVE &dn)
		str name.ansi(dn); CoTaskMemFree dn
		 out name
		if(!matchw(name filesToCopy 1)) continue
	 add to the 'copy' collection
	fo.CopyItem(si siFolder2 0 0)
	R+1

 now copy
if(R) fo.PerformOperations

ret R

 TODO: options, eg overwrite existing files without asking.
