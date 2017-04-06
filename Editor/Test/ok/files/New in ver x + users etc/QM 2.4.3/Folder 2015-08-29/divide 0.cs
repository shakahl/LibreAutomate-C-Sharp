dll "qm.exe" MemExc
out 1
 sub.Test
_i/0
 MemExc


#sub Test
out 2
 min 0
 err out _error.description
 _i/0
 err out _error.description
MemExc
 err out _error.description


 #sub Test2
 sub.Test
