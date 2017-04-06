str folder="My QM"
str filename="test.php" ;;not tested: don't know whether it always must be with extension or as displayed

int w=win(folder "CabinetWClass"); if(!w) end ERR_WINDOW
act w
SHDocVw.InternetExplorer ie=GetFolderWindowIE(w); if(!w) end ERR_FAILED
Shell32.ShellFolderView d=ie.Document

 find and select folder item whose name is filename
Shell32.FolderItem fi
foreach fi d.Folder.Items
	str name=fi.Name
	if name~filename
		VARIANT v=fi
		d.SelectItem(v 1|4|8|16) ;;https://msdn.microsoft.com/en-us/library/windows/desktop/bb774047%28v=vs.85%29.aspx
		break
