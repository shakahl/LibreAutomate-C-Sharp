 /
function action str&sv $sl

 Initializes listbox or combobox variable so that action would be selected.


if(action<1) sv.from("&" sl); ret
ARRAY(str) a=sl
if(action>=a.len) action=0
a[action]-"&"
sv=a
