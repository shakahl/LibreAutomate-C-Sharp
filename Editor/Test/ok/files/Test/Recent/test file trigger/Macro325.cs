 str s1="E:\Software\Copy (5) of Njw t.txt"
 str s2="E:\Software\New Folder\Copy (5) of Njw t.txt"
 
  s1.GetShortPath
  s2.GetShortPath
  s1.GetLongPath
  s2.GetLongPath
 
 del- s2; err
 cop- s1 s2

spe
 ren* "E:\Software\Njpt.txt" "AAAA.txt"
 ren* "E:\Software\AAAA.txt" "Njpt.txt"

str s="aaaaaaaaaa"
rep(5) s.setfile("E:\Software\Njpt.txt");; 0.03

 __HFile f
  f.Create("E:\Software\Njpt.txt" OPEN_EXISTING)
 f.Create("E:\Software\Njpt.txt" CREATE_ALWAYS)
 WriteFile(f s s.len &_i 0)
 f.Close
