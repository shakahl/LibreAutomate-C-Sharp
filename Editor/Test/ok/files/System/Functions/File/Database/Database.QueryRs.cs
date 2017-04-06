function# $sql ADO.Recordset&rs [flags] ;;flags: 1 disconnect recordset

 Executes SQL query (usually SELECT) and gets data into ADO.Recordset object.
 Returns the number of retrieved records.

 rs - variable for data.

 REMARKS
 Use this function instead of QueryArr if you need ADO.Recordset object.
 ADO.Recordset object is used as container for retrieved data. It can have multiple records (rows) and fields (cells).
 You can use its functions to get/set data and various properties, such as field data types.
 To easily get data from a recordset, you can use Database functions RsGetValue, RsGetRecord and RsGetAll.


if(!rs) rs._create
rs.Open(sql conn ADO.adOpenStatic ADO.adLockBatchOptimistic -1)
rs.MarshalOptions=ADO.adMarshalModifiedOnly
if(flags&1) IDispatch c0; rs.ActiveConnection=c0
ret rs.RecordCount

err+ end _error
