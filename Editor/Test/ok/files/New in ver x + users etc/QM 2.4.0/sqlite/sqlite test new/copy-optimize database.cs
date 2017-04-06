int w=win("SQLite2009 Pro - The Best GUI to manage your SQLite3 databases." "ThunderRT6FormDC"); if(w) clo w
str sf="$my qm$\test\ok.db3"
if(!ConvertQmlToSqlite("$qm$\ok.qml" sf 1024*0)) out "FAILED"

if(dir(sft)) del- sft
PF
Sqlite x.Open(sft 0 2)
PN
ARRAY(str) at
x.Exec("SELECT type,name,sql FROM sqlite_master WHERE type='table' AND name IN('items','texts')" a)
PN
 x.Exec("PRAGMA foreign_keys=ON")
x.Exec("BEGIN")
str sql=
F
 CREATE TABLE main.file AS SELECT * FROM src.file;
 CREATE TABLE main.items AS SELECT * FROM src.items LIMIT 0;
 CREATE TABLE main.texts AS SELECT * FROM src.texts LIMIT 0;
 INSERT INTO items(id,name) VALUES(1, 'nnn');
 INSERT INTO items(id,name) VALUES(1, 'nnn');
x.Exec(sql)
x.Exec("END")
PN;PO

run sf
