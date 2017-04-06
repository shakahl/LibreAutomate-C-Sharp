DIALOG d
d.add(id "Class" style exstyle x y cx cy text)
d.add(id "Class" style exstyle x y cx cy text)
d.add(id "Button" style exstyle x y cx cy text)
...
d.set(id value)
...
d.show(&dlgproc hwndowner flags styleadd styleremove param x y)
d.get(id var1)
___________________________

in dlgproc:

DIALOG d.fromhdlg(hDlg)
d.get(id var1)
d.set(id value)
