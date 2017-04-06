 /
function $_file [flags] ;;flags: 1 only file

 Compacts and defragments a SQLite database file.

 REMARKS
 When modifying a SQLite database (INSERT, UPDATE, DELETE), quite soon it becomes fragmented. It slows down queries and other operations, possibly 100 or more times. Also, file does not shrink when data is deleted.
 The fragmentation has 2 levels: file system fragmentation and internal database fragmentation. Both levels has much impact on speed.
 This function fixes all the above problems, and restores the initial speed.


opt noerrorshere 1
str sf.expandpath(_file) sft.from(sf ".defrag")

int att=GetAttr(sf); if(att&FILE_ATTRIBUTE_READONLY) end "read-only file"

if(!CopyFileW(@sf @sft 0)) end ERR_FAILED 16

if flags&1=0
	Sqlite x.Open(sft 0 1)
	x.Exec("PRAGMA journal_mode=OFF;VACUUM")
	x.Close

 if(!ReplaceFileW(@sf @sft 0 0 0 0)) end ERR_FAILED 16
if(!MoveFileExW(@sft @sf MOVEFILE_REPLACE_EXISTING)) end ERR_FAILED 16
