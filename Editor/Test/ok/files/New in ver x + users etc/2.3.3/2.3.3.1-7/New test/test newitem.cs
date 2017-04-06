 newitem "private" "" "\ff\Old\private"

 int iidF=qmitem("\ff\Old\private")
 newitem "private" "" iidF

 newitem "bbbb" "g" "test newitem"

 newitem "bbbb" "$desktop$\test.txt" "tsm.cpp"

 newitem "bbbb" "$desktop$\test.txt" "File Link"

 QMITEM q
 int iidF=qmitem("\ff\Old\private" 0 q 4)
 int f=newitem("private" "" iidF q.programs)
 newitem "" "mes 1[]" "" "F12" f

int f=newitem("F" "" "Folder" "\QM")
newitem "" "mes 1[]" "" "F12" f
