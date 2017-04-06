str sk="Test\Del"
 str sk="Test\Del\Del2"
rset sk "val" sk
int hr

 hr=RegDeleteKey(HKEY_CURRENT_USER sk)

 hr=SHDeleteKey(HKEY_CURRENT_USER sk)

 rset "" "val" sk 0 -1
 hr=SHDeleteEmptyKey(HKEY_CURRENT_USER sk)

dll "qm.exe" !_RegKeyDelete $key $subkey hive flags

 _i=_RegKeyDelete("Test" "Del" 0 0)
_i=_RegKeyDelete("Test\Del" "" 0 0)
if(!_i) hr=GetLastError
out _i

if(hr) out hr; out _s.dllerror("" "" hr)

 out rset("" "Del" "Test" 0 -2)
 out rset("" "Del" "Test" 0 -2)
 out rset("" "" "Test\Del" 0 -2)
