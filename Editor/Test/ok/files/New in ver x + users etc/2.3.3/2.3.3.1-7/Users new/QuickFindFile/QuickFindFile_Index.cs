 /
function $databaseFile $folders

 Creates database for <tip>QuickFindFile_Find</tip>.
 Error if fails.

 databaseFile - database file. Ex: "$my qm$\x.db3"
 folders - list of folders. Will get all file and folder paths from these folders. Ex: "C:[]E:\Folder"


Sqlite db.Open(databaseFile)
db.Exec("DROP TABLE files"); err
db.Exec("BEGIN TRANSACTION")
db.Exec("CREATE TABLE files (path)")

str f
foreach f folders
	f+iif(f.end("\") "*" "\*")
	Dir d
	foreach(d f FE_Dir 0x6)
		str sPath=d.FileName(1)
		sPath.lcase ;;in Sqlite LIKE, Unicode chars case sensitive
		sPath.SqlEscape
		db.Exec(F"INSERT INTO files VALUES ('{sPath}')")

db.Exec("END TRANSACTION")

err+ end _error

 info: with sqlite 50% slower than with raw txt file
