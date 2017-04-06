out
str folder
 folder="nofolderr"
 folder=""
 folder="\Main"
folder="\System"
int flags=0
str s1 s2

PF
EnumQmFolder_old folder 1 &EQF_test &s1
PN
out s1; out "-------------"

EnumQmFolder folder 1 &EQF_test &s2
PN;PO
out s2

out s2=s1


