out
ARRAY(str) a
 GetFilesInFolder a "$qm$" "" 4
 GetFilesInFolder a "$qm$" "" 4|0xC0000 0 "(?<=^|\\)(Debug\\|Release\\)"
 GetFilesInFolder a "$qm$" "*.png[]*.gif[]*.jpg" 4
 GetFilesInFolder a "$qm$" "" 4 0 "*.cpp[]*.c[]*.h[]*.qml"
GetFilesInFolder a "$qm$" "*x*[]*v*" 4 0 "*.cpp[]*.c[]*.h[]*.qml"
out a
out a.len
