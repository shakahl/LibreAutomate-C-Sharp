function hgrid $sql [flags] ;;flags: 1 except first column, 2 append

 Populates QM_Grid control with data from database.
 Error if something fails, eg if SQL is incorrect.
 Error if column count in query results is greater than grid column count minus flag 1.

 hgrid - QM_Grid control handle or DlgGrid variable.
 sql - SQL statement that selects data to be displayed in the grid. For example, "SELECT * FROM table1" displays whole table1.
 flags:
   1 - don't add data to the fist column.
   2 (QM 2.3.3) - append (don't clear previous contents).

 <open "sample_Grid_Sqlite">Example</open>


type ___SQLITEQG DlgGrid'g ncol firstcol nr ~es

___SQLITEQG x.g.Init(hgrid)

x.firstcol=flags&1
x.ncol=x.g.ColumnsCountGet-x.firstcol

if(flags&2) x.nr=x.g.RowsCountGet
else x.g.RowsDeleteAll(x.firstcol)

ExecF(sql &sub.Callback &x)
err
	if(x.es.len) end x.es
	end _error


#sub Callback
function[c]# ___SQLITEQG&x ncol $*data $*colNames

if(ncol>x.ncol) x.es="column count does not match"; ret 1
if(x.g.RowAddSet(x.nr 1 data ncol x.firstcol 1)<0) x.es="failed"; ret 1
x.nr+1

err+ ret 1
