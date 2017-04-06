out
str folder
 folder="nofolderr"
folder=""
 folder="\Main"
 folder="\System"
int flags=0
str s1 s2

 PF
MenuFromQmFolder_old folder s1 flags
 PN
 out s1
DynamicMenu(s1 "" 1)

MenuFromQmFolder folder s2 flags
 PN;PO
 out s2
DynamicMenu(s2 "" 1)

out s2=s1
