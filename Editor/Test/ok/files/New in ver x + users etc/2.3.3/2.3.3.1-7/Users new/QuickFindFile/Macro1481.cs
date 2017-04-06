out

str dbFile="$my qm$\QuickFindFile_Index.txt"

int t1=timeGetTime

QuickFindFile_Index2 dbFile "$qm$[]$my qm$"
 QuickFindFile_Index2 dbFile "c:"

int t2=timeGetTime

ARRAY(str) a
QuickFindFile_Find2 dbFile "*.chm" a

int t3=timeGetTime

out "%i %i" t2-t1 t3-t2

out a
