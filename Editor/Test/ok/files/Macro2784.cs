int w=win("Dialog" "#32770")
int c=id(3 w) ;;editable text
str s.all(1000)
out SendMessage(c WM_GETTEXT 1000 s)
