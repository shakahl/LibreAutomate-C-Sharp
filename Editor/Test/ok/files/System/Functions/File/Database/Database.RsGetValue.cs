function ADO.Recordset'rs str&s [`column] [record]

 Gets value of a field of recordset.

 s - variable for value.
 column - column name or 0-based index. Default: 0.
 record - 0-based record (row) index.


if(!column.vt) column=0
rs.MoveFirst; if(record) rs.Move(record)
s=rs.Fields.Item(column).Value; err s.all

err+ end _error
