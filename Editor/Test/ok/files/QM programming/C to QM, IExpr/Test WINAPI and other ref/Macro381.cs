 WINAPI.IShellDispatch4 p._create(WINAPI.CLSID_ShellDispatchInproc)
WINAPI.IShellDispatch sd._create(WINAPI.CLSID_Shell)
WINAPI.Folder f
sd.BrowseForFolder(_hwndqm "ti" 0 @ f)
WINAPI.FolderItems fi
f.Items(fi)
WINAPI.FolderItem i
foreach i fi
	BSTR b
	i.get_Path(b)
	out b
 IStringMap m.Add
