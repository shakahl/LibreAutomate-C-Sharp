function ADO.Recordset'rs ARRAY(str)&a [record]

 Gets values of all fields of single record (row) of recordset.

 a - variable for values.
 record - 0-based record index.


ADO.Field field
rs.MoveFirst; if(record) rs.Move(record)
a.create(rs.Fields.Count)
foreach(field rs.Fields)
	a[_i]=field.Value; err
	_i+1

err+ end _error
