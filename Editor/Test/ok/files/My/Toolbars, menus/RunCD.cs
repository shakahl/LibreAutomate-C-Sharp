ARRAY(Wsh.Drive) a; int i j; str s
GetDrives a 4
if(!a.len) ret
run a[0].Path
