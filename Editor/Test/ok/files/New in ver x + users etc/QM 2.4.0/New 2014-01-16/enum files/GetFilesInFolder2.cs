SetCurDir "$My QM$\test"
out
ARRAY(str) a
GetFilesInFolder a "$My QM$\test" "" 2|4
 GetFilesInFolder a "$My QM$\test" "" 2|8|0x100
 GetFilesInFolder a "$My QM$\test" "*.qmL" 2|4
 GetFilesInFolder a "$My QM$\test" "export.qmL" 2|4
 GetFilesInFolder a "$My QM$\test" ".+-" 2|4|0x10000
 GetFilesInFolder a "$My QM$\test" "il2\*" 0x20006
 GetFilesInFolder a "$My QM$\test" "" 2|4 1
 GetFilesInFolder a "" "*.qml" 2|4
out a
