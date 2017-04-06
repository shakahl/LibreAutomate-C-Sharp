int w1=win("Dialog" "#32770")
str s
out s.getwinexe(w1 0)
out s.getwinexe(w1 1)

out ProcessNameToId(s)
out ProcessNameToId("abcdefghijkl.exe")
out ProcessNameToId("abcdefghijkl")
out ProcessNameToId(s 0 1)
out ProcessNameToId("abcdefghijkl.exe" 0 1)
out ProcessNameToId("abcdefghijkl" 0 1)
