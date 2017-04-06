typelib STI "%com%\UI\STI (tray)\STI.ocx"
STI.STI t._create
opt waitmsg 1
t._setevents("t__DSTIEvents")
 t.IconFile=_s.expandpath("$qm$\deb next.ico")
t.IconType=1
t.Appear
10
