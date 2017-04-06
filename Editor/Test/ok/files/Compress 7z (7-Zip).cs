str sevenZip.expandpath("$qm$\7z.exe") ;;from c:\program files\7-zip\
 RunConsole2(F"''{sevenZip}'' -?"); ret

str file1="$qm$\qm.exe"
str file2="$my qm$\qm.7z"
file1.expandpath
file2.expandpath
out RunConsole2(F"''{sevenZip}'' a ''{file2}'' ''{file1}''")

 qm.exe - 1447 KB
 qm.zip - 791 KB
 qm.7z - 644 KB
