MountVolume("D:" "$desktop$\D")
 
str s.GetMountPointTarget("$desktop$\D")
out s
 5
 UnmountVolume "$desktop$\D"



 CreateSymLink "$desktop$\test_sl.txt" "$desktop$\test.txt"
 
 str s.GetSymLinkTarget("$desktop$\test_sl.txt")
 out s
  5
  del "$desktop$\test_sl.txt"
