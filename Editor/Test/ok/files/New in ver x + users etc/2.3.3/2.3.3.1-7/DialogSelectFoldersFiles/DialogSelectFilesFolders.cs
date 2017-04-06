 /
function! str&s [hwndOwner]

 Shows simple dialog where you can enter paths of files or folders.
 You can use hotkey to select files from Windows Explorer.
 Returns 1 on OK, 0 on Cancel.

 s - variable that receives paths.
 hwndOwner - owner window handle.

 EXAMPLE
 str sFiles sFile
 if(!DialogSelectFilesFolders(sFiles)) ret
 foreach sFile sFiles
	 out sFile


s=""
str controls = "3"
str e3
if(!ShowDialog("DSFF_Dialog" &DSFF_Dialog &controls)) ret
s=e3
ret 1
