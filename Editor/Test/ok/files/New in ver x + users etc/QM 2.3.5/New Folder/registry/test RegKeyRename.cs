str sk="Test\Del"
 str sk="Test\Del\Del2"
rset sk "val" sk

dll "qm.exe" !_RegKeyRename $key $newName hive !deleteExisting

int n
n=_RegKeyRename("Test\Del" "Ren" 0 0)
 n=_RegKeyRename("Test\Del" "Ren" 0 1)
 n=_RegKeyRename("Test" "Ren" 0 0)
 n=_RegKeyRename("Test" "Ren" 0 1)

if(!n) out _s.dllerror("" "ntdll"); outx GetLastError
