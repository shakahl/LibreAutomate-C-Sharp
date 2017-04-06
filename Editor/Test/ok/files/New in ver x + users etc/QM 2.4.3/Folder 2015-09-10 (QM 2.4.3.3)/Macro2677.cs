dll "Qm.exe" #RxTest $s1 $s2 fl

out RxTest("one t2 three", "T\d", 7)

findrx("" "T\d" 0 129 _s)
 outb _s 4 1
out RxTest("one t2 three", _s 9)
