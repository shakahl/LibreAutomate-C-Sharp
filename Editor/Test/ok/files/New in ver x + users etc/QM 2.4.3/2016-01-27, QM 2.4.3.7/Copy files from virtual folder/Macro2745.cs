out

Shell32.Shell x._create
Shell32.Folder fMC=x.NameSpace(Shell32.ssfDRIVES) ;;My Computer
Shell32.FolderItem fi
foreach fi fMC.Items
	str name=fi.Name
	out name
