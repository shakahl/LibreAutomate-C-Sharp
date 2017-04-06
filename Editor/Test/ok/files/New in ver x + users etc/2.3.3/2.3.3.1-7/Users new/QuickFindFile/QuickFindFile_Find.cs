 /
function $databaseFile $filePattern ARRAY(str)&results

 Finds files in database created by <tip>QuickFindFile_Index</tip>.
 Error if fails.

 databaseFile - database file.
 filePattern - file pattern. Must match full path. Examples: "*.txt", "C:\*.txt", "C:\Folder\*", "*\file.txt".
 results - receives full paths of found files and folders.


Sqlite db.Open(databaseFile)

str s=filePattern
s.lcase
s.findreplace("`" "``")
s.findreplace("%" "`%")
s.findreplace("_" "`_")
s.findreplace("*" "%")
s.findreplace("?" "_")
s.SqlEscape
db.Exec(F"SELECT path FROM files WHERE path LIKE '{s}' ESCAPE '`'" results)

err+ end _error

 info: with sqlite 5 times slower than with raw txt file. Not faster if we get all and use matchw.
