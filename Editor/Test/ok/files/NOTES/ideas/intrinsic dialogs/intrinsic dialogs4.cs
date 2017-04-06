 BEGIN DIALOG
 id "class" style exstyle x y w h "text"
 ...
 END DIALOG

type MYDLG :DIALOG var1 ...

MYDLG d.show("dd" &dlgproc hwndowner flags styleadd styleremove param x y); err ret
out d.var1

 dd can be empty to load from this func, or macro name or DD string
___________________________

in dlgproc:

MYDLG d.fromhdlg(hDlg)
d.var1=value
out d.var1


ret d.ret(value)
___________________________

Can use predefined types and DD:

DLG_USERPASSWORD d.show(DD_USERPASSWORD); err ret
out d.user
out d.password
