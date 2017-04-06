 /
function str'folder [flags] [$tb] ;;flags are the same as with FE_Dir

 Shows menu of files in folder. Menu is not recursive, although can retrieve files from subfolders if flag 4 is set.
 folder can be simple, eg "C:\My Folder", or special folder, eg "$Desktop$".
 folder can include file pattern, eg "$Desktop$\*.txt" (only txt files).
 tb - if used, creates toolbar instead of menu. tb then must be temporary toolbar name.

 EXAMPLE
 MenuFromFileFolder "$Personal$"


if(findc(folder '*')>=0)
else if(folder.end("\")) folder+"*"
else folder+"\*"

str s
Dir d; str sPath sFn
foreach(d folder FE_Dir flags&15)
	sFn=d.FileName; sFn.escape(1)
	sPath=d.FileName(1); sPath.escape(1)
	s.formata("%s :run ''%s''[]" sFn sPath)

if(!empty(tb)) DynamicToolbar(s tb)
else DynamicMenu(s)
