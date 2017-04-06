 Shows all changed/added/removed items in a folder of the current qml file, compared with another qml file (all file items, not just that folder).

 str sFolder="\System"
str itemInFolder="init" ;;System
 str sFolder="\Archive"
str sFile="Q:\app - new, VS2015, broken\system.qml" ;;old file; change this

out
 str s.getfile(sFile)
EnumQmFolder sFolder 1|8 &CQF_proc &s

Sqlite& xnew=_qmfile.SqliteBegin(qmitem(itemInFolder))
Sqlite xold.Open(sFile 1)

ARRAY(QMITEMIDLEVEL) a
if(!GetQmItemsInFolder(sFolder a)) end "no folder"
int i
for i 0 a.len
