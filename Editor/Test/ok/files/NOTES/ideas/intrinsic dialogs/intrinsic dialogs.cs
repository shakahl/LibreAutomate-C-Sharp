str var1; int var2
DIALOG d
d.add(var1 id "Class" style exstyle x y cx cy text)
d.add(0 id "Class" style exstyle x y cx cy text)
d.add(var2 id "Button" style exstyle x y cx cy text)
...
d.show(&dlgproc hwndowner flags styleadd styleremove param x y)
out var1
out var2
___________________________

in dlgproc:

DIALOG d.fromhdlg(hDlg)
d.get(id var1)
d.set(id value)
