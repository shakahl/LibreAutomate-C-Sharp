function$ [$fileExt] [$fileName] [$folder] [str&fileData]

 Creates full path for a temporary file and stores it in this variable. The file will be auto-deleted later.
 Returns the path. Error if fails.

 fileExt - file extension, like "txt" or ".txt". Optional.
 fileName - filename. Can be with or without extension.
   If empty, creates random filename.
 folder - parent folder.
   If empty, uses default folder "$temp qm$". Normally it is "$temp$\QM". In portable QM it is "$temp$\QM-portable"; portable QM deletes it at exit.
   If begins with "\", it is subfolder in default folder.
   In any case, creates the folder if does not exist.
 fileData - if used, the function creates the file and saves the data there.

 REMARKS
 The file will be deleted when destroying this variable. For example, if it is a local variable, deletes when the function exits.
 Can be used for temporary folders too.
 This variable contains full path of the file, and can be used as str variable.

 See also: <__TempFile.Delete>


opt noerrorshere 1

str sf sn

if empty(folder) or folder[0]='\'
	lpstr f="$temp qm$"
	folder=iif(empty(folder) f sf.from(f folder))
sf.expandpath(folder)

if empty(fileName)
	sn.Guid
	fileName=sn

lpstr dot; if(!empty(fileExt) and fileExt[0]!'.') dot="."

m_file.from(sf "\" fileName dot fileExt)

mkdir sf
if(&fileData) fileData.setfile(m_file)

ret m_file
