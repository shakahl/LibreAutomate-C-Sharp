 /bug return interface from array
function'Wsh.Drive

ARRAY(Wsh.Drive) a
GetDrives a 2

 outref a[0]
 out a[0].AddRef
ret a[0]

Wsh.Drive d=a[0]
  out d.AddRef
ret d
