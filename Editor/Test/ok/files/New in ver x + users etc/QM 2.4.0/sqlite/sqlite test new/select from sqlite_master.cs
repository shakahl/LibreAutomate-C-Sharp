str sf="$my qm$\test\ok.db3"
Sqlite x.Open(sf 0 2)
ARRAY(str) at; int i
x.Exec("SELECT type,name,sql FROM sqlite_master WHERE type='table' AND name IN('items','texts')" a)
for(i 0 at.len) out at[2 i]
