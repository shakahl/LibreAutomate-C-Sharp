function str'folder [flags] ;;flags: 0 files, 1 folders, 2 any, 4 include subfolders, 8 only subfolders

 Creates list of files that are in folder, formatted like a QM menu.
 The list can be used with DynamicMenu or DynamicToolbar.
 If this variable is not empty, appends. For example it can contain line with toolbar options.
 folder can be simple, eg "C:\My Folder", or special folder, eg "$Desktop$". Can be file pattern, eg "$Desktop$\*.txt".

 EXAMPLE
 str s=" /siz 60 300[]"
 s.MenuFromFileFolder("$Personal$")
 DynamicToolbar s "tbff"


if(findcs(folder "*?")>=0)
else if(folder.end("\")) folder+"*"
else folder+"\*"

Dir d; str sPath sFn
foreach(d folder FE_Dir flags&15)
	sFn=d.FileName; sFn.escape(1)
	sPath=d.FileName(1); sPath.escape(1)
	this.formata("%s :run ''%s''[]" sFn sPath)
