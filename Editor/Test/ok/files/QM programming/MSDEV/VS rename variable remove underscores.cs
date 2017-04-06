 In Visual Studio, takes word from mouse, and converts var_name_etc to varNameEtc.

dou
str s.getsel
 str s="_var_bar_mur_16_"
REPLACERX r.frepl=&sub.Callback_str_replacerx
if(!s.replacerx("\B_\w" r)) ret
 out s
key CN2 ;;must be assigned this hotkey to VAssistX.Rename
int w=wait(30 WA win("Rename" "#32770"))
wait 0 WE id(1176 w) ;;push button 'Rename'
key (s) Y


#sub Callback_str_replacerx
function# REPLACERXCB&x

x.match.remove(0 1)
x.match.ucase(0 1)
