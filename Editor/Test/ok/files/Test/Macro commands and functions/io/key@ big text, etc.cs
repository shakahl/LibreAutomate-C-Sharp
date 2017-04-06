 test with and without '...use SendInput' checked
str s.getfile("$qm$\license.txt")
run "notepad"
BlockInput 1
key (s)
key A{ea} Cx
1
'Cv
BlockInput 0
