 ARRAY(Wsh.Drive) a
 GetDrives a
  out a[1].AvailableSpace/1024/1024
 out a[1].FileSystem
 out a[0].IsReady

ARRAY(Wsh.Drive) a; int i j; str s
GetDrives a
for i 0 a.len
	s=a[i].DriveLetter
	if(a[i].IsReady)
		j=a[i].AvailableSpace/1024/1024 ;;free space available to the current user
		out "%s free space=%i MB" s j
	else out "%s not ready" s

 ARRAY(Wsh.Drive) a; int i; str s; long j
 a=GetDrives()
 for i 0 a.len
	 s=a[i].DriveLetter
	 j=a[i].FreeSpace
	 out "%s free space=%I64i MB" s j/1024/1024
