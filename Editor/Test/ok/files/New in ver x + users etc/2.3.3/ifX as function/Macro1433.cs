out
str s="$desktop$\test.txt"
out iff("notepad.exe")
out iff(s)
out iff("$desktop$\nofile.txt")

if(iff("notepad.exe") and iff(s) and !iff("$desktop$\nofile.txt")) out 8
out iff("notepad.exe") + iff(s) + iff("$desktop$\nofile.txt")

 Can be used as function
if(iff("file1") and !iff("file2")) out "file1 exist; file2 does not exist."
