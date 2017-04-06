function ADO.Recordset'rs ARRAY(str)&a

 Gets all data from recordset into array.

 a - variable for data. The array will have 2 dimensions.


int r f nr nf
ADO.Field field
rs.MoveFirst
nr=rs.RecordCount; nf=rs.Fields.Count
a.create(nf nr)
for r 0 nr
	f=0
	foreach(field rs.Fields)
		a[f r]=field.Value; err
		f+1
	rs.MoveNext

err+ end _error
