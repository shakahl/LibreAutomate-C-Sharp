out
 str folderFiles="$my qm$\*.txt"
str folderFiles="$documents$\*.txt"

str sm
ARRAY(str) a
ShowMenuStringFromFilesInFolder folderFiles sm a
out "-----------"
out sm
int i=ShowMenu(sm)-1
if(i<0) ret
out a[i]
