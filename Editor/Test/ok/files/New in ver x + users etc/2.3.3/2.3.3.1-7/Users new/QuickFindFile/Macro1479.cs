out

str dbFile="$my qm$\QuickFindFile_Index.db3"

int t1=timeGetTime

QuickFindFile_Index dbFile "$qm$[]$my qm$"
 QuickFindFile_Index dbFile "c:"

int t2=timeGetTime

ARRAY(str) a
QuickFindFile_Find dbFile "*.chm" a

int t3=timeGetTime
out "%i %i" t2-t1 t3-t2

for(_i 0 a.len) out a[0 _i]
