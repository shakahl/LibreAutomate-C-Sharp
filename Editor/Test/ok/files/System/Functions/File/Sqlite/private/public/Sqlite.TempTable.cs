function $name [$template_table]

 Creates temporary empty table with same structure as of template_table.
 At first drops the temporary table if exists.
 If template_table omitted or "", just drops the temporary table if exists.


Exec(F"DROP TABLE IF EXISTS [{name}]")
if(empty(template_table)) ret

Exec(F"CREATE TEMP TABLE [{name}] AS SELECT * FROM [{template_table}] LIMIT 0")

err+ end _error

 note: fbc this is public, just hidden.
