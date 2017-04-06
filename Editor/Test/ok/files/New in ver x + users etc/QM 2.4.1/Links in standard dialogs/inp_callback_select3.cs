 /
function# hwnd $s

str items=
 one
 two
 three
int i=ShowMenu(items hwnd 0 2)
if(!i) ret
str si.getl(items i-1)
EditReplaceSel hwnd 4 si 3
