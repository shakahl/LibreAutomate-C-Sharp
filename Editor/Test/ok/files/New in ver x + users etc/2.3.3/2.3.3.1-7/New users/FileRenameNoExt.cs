 /
function $oldFilePathName $newNameWithoutExt

 Renames file, leaving same extension.

 oldFilePathName - full path of existing file.
 newNameWithoutExt - new file name, without path and without extension.

 EXAMPLE
 FileRenameNoExt "$desktop$\test.txt" "test2"


ren* oldFilePathName _s.from(newNameWithoutExt PathFindExtension(oldFilePathName))
err end _error
