str sf="$my qm$\test\ok.db3"
if(!ConvertQmlToSqlite("$qm$\ok.qml" sf 1024*0)) out "FAILED"
Sqlite y.Open(sf 0 2); y.Exec("CREATE INDEX items_ord ON items(ord)")
