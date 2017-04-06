out
str sm; ARRAY(str) a
ShowMenuStringFromFilesInFolder2 "$myqm$\*.txt" sm a
out sm;ret
int i=ShowMenu(sm)-1; if(i<0) ret
out a[i]
