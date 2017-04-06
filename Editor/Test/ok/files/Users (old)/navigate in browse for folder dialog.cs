int w1=win("Browse For Folder" "#32770")
act w1
int edit=child("" "Edit" w1)
str s="c:\windows"
s.setwintext(edit)
but 1 w1
