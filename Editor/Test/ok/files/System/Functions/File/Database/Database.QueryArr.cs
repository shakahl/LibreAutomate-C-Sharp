function# $sql ARRAY(str)&a [flags] ;;flags: 1 single record (1-dim array)

 Executes SQL query (usually SELECT) and gets data into array.
 Returns the number of retrieved records. Returns 0 if not found.

 a - variable for data.
   By default, the array will have 2 dimensions. Use flag 1 if the query retrieves only single record and you need 1-dim array.


ADO.Recordset rs
int n=QueryRs(sql rs)
if(!n) a=0; ret
if(flags&1) RsGetRecord(rs a)
else RsGetAll(rs a)
ret n

err+ end _error
