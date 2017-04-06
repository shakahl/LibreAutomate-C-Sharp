 run "C:\Users\G\AppData\Roaming\Microsoft\Windows\SendTo"

 register
out RegisterComComponent("$qm$\qmshex32.dll" 8)
 out RegisterComComponent("$qm$\qmshex32 - Copy.dll" 8)

 unregister
 out RegisterComComponent("$qm$\qmshex32.dll" 8|1)
 out RegisterComComponent("$qm$\qmshex32 - Copy.dll" 8|1)


 out _s.dllerror("" "" 0x80020009)
 out _s.dllerror("" "" 0x80070057)
 out _s.dllerror("" "" 0x80040111)

 SHChangeNotify SHCNE_ASSOCCHANGED SHCNF_IDLIST 0 0
