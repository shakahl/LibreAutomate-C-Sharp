 /
function[c]# param ncol $*data $*colNames

 Callback function for Sqlite.ExecF.

 param - param of Sqlite.ExecF. Can be declared as pointer or reference to any type.
 ncol - number of columns in the row of results.
 data - pointer-based array of column data strings of the row. Read-only.
 colNames - pointer-based array of column names. Read-only.

 Return: 0 continue, 1 error.


 out len(data[0])
