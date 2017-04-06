 BEGIN DIALOG
 var1 id "class" style exstyle x y w h "text"
 ...
 END DIALOG

type MYDLG :DIALOG var1 ...

MYDLG d
if(!showdialog(d "dd" &dlgproc hwndowner flags styleadd styleremove param x y)) ret
out d.var1

 dd can be empty to load from this func, or macro name or DD string
___________________________

in dlgproc:

MYDLG d.fromhdlg(hDlg)
d.getall
out d.var1
d.var1=value
d.setall

ret d.ret(value)
___________________________

Can use predefined types and DD:

DLG_USERPASSWORD d.show(DD_USERPASSWORD); err ret
out d.user
out d.password
