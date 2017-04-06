function hgrid $table [flags] [$template_table] ;;flags: 1 no first column, 2 no empty rows, 4 selected/checked

 Populates table with data from QM_Grid control.
 Error if something fails.

 hgrid - QM_Grid control handle or DlgGrid variable.
 table - table name. Unless template_table used, the table must exist, and its column count must be equal to the grid column count minus flag 1.
 template_table - if used, table will be created as temporary table with the same structure as of template_table.

 <open "sample_Grid_Sqlite">Example</open>


str sql cell.flags=1
DlgGrid g.Init(hgrid)
int r c col1(flags&1) ncol(g.ColumnsCountGet-col1) nb
if(ncol<1) end "no data columns in grid control"

Exec("BEGIN")
int trans=1

if(!empty(template_table)) TempTable(table template_table)
else Exec(F"DELETE FROM [{table}]")

for r 0 g.RowsCountGet
	lpstr s=g.RowGetMS(r ncol col1 flags&6 nb)
	if(!nb) end F"failed to get row {r}" ;;todo: rollback?
	if(nb<0) continue ;;flag 2 or 4
	sql=F"INSERT INTO [{table}] VALUES("
	for c 0 ncol
		cell=s; s+len(s)+1
		sql.formata("'%s'," cell.SqlEscape)
	sql[sql.len-1]=')'
	 out sql
	Exec(sql)

Exec("COMMIT")

err+
	QMERROR e=_error; e.description.formata("[][]Last SQL statement: %s" sql)
	if(trans) Exec("ROLLBACK"); err
	end e
