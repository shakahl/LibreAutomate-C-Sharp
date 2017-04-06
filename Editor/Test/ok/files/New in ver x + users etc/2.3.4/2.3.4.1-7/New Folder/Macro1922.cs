run "$system$\notepad.exe"
 run "$system$\notepad.exe" "" "" "" 0x30000

 outx GetFileAttributes("%windir%")

 out _s.expandpath("%windir%")
