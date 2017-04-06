 Enumerates items in desktop and first level subfolders

Shell32.Shell sh._create
Shell32.Folder f f2
Shell32.FolderItem fi fi2

f=sh.NameSpace(Shell32.ssfDESKTOP)

foreach fi f.Items
	out fi.Name
	if(fi.IsFolder)
		f2=fi.GetFolder
		foreach fi2 f2.Items
			str s=fi2.Name
			out "[9]%s" s
