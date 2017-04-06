 Displays full paths of selected files and folders in Windows Explorer.

 find Windows Explorer window
int hwnd=win("" "ExploreWClass")
if(!hwnd) hwnd=win("" "CabinetWClass")
if(!hwnd) end "folder window not found"
 Get ShellBrowserWindow interface by enumerating shell windows
SHDocVw.ShellWindows sw._create
SHDocVw.ShellBrowserWindow b
foreach(b sw)
	int hwnd2=b.HWND; err continue
	if(hwnd=hwnd2) goto g1
ret
 g1
 get shell folder view document
Shell32.ShellFolderView sfw=b.Document
Shell32.FolderItem fi
 enumerate selected items
foreach fi sfw.SelectedItems
	out fi.Path
