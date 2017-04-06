 Functions of Dir class are used to enumerate files and get file info (path, attributes, size, times).
 To enumerate files, use foreach and FE_Dir. It initializes the variable. See example.
 To initialize the variable for a single file, call function dir of Dir class. See example.
 Other Dir functions return file properties that are stored in the variable (retrieved when initializing it with FE_Dir or dir).

 To insert code, you can use dialog "Enumerate files". Tips:
   In "Folder\pattern" field, use folder path and filename pattern. The filename pattern can contain <help #IDP_WILDCARD>wildcard characters</help> * and ?.
   For example, "C:\My Documents\*.txt" matches all text files in "C:\My Documents" folder.

 See also: <GetFilesInFolder>, <FileGetAttributes>.

 EXAMPLES
Dir d
if d.dir("$desktop$\test.txt") ;;if file exists
	str sPath=d.FullPath
	out sPath
	DateTime dt=d.TimeModifiedLocal ;;get time modified
	out dt.ToStr(4)

Dir d2
foreach d2 "$qm$\*" FE_Dir ;;for each file in QM folder
	str sPath2=d2.FullPath
	out sPath2
	DateTime dt2=d2.TimeModifiedLocal ;;get time modified
	out dt2.ToStr(4)
