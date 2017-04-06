 /
function $name $_class [$exename] [flags] [$prop]

 Closes all matching windows.

 All parameters are the same as with <help>win</help>.

 Added in: QM 2.3.0.
 QM 2.4.3: added prop.


spe -1
ARRAY(int) a; int i
win(name _class exename flags prop a)
for(i 0 a.len) clo a[i]; err
