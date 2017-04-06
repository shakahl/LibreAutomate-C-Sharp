out
if(!CopyFile("Q:\my qm\test\ok.QML" "\\gintaras\q\my qm\test\n.QML" 0)) end _s.dllerror
if(!CopyFile("\\gintaras\q\my qm\test\ok.QML" "Q:\my qm\test\n.QML" 0)) end _s.dllerror

out GetAttr("Q:\my qm\test\n.QML:wal"); err out _error.description
out GetAttr("\\gintaras\q\my qm\test\n.QML:wal"); err out _error.description
