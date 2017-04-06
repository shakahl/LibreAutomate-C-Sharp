 BEGIN DIALOG
 var1 id "class" style exstyle x y w h "text"
 ...
 END DIALOG

d.show(dd &dlgproc hwndowner flags styleadd styleremove param x y)
out d.var1

 dd can be empty to load from this func, or macro name or DD string
___________________________

in dlgproc:

DIALOG d.fromhdlg(hDlg)
d.var1=value
out d.var1


ret d.ret(value)
