 dll "qm.exe" RandomTest mn mx

 RandomTest 0 0x7fffffff

 out "0x%X" RandomInt3(0 0x7fffffff)

out RandomInt(0 10)
 out RandomInt(0 -10)
 out RandomInt3(-10 -20)
 out RandomInt3(-10 0)
 out RandomInt3(10 10)
 out RandomInt3(30 10)
 out "-----"

 out RandomDouble2(-10 0)
 out RandomDouble2(0 -10)
 out RandomDouble2(-10 -20)
 out RandomDouble2(30 10)
