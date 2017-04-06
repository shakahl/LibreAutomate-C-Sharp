 /
function! $sharedFolder str&filePath

 Gets QM shared folder file path.
 Returns: 1 success, 0 failed.

 sharedFolder - shared folder path (like "\System"), or GUID (like "{ACBAA175-3536-4603-9C7D-BF35F590075A}") or QM item id (like +iid).
 filePath - this variable receives file path of the shared folder.

 EXAMPLE
 str s
 if(!GetQmSharedFolderFilePath("\System" s)) end "failed"
 out s


int rowid; _qmfile.SqliteItemProp(sharedFolder rowid)
Sqlite& x=_qmfile.SqliteBegin
SqliteStatement k.Prepare(x F"SELECT td FROM items WHERE id={rowid}")
if(!k.FetchRow) ret
lpstr s=k.GetBlob(0 &_i); if(_i<5) ret
_qmfile.SqliteEnd
s+4
if(findc(s '\')<0) filePath.from(_qmdir s); else filePath=s; filePath.expandpath
ret 1
err+
