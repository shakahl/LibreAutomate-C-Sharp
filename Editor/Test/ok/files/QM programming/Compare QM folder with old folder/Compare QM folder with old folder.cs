 Shows all changed/added/removed items in a folder of the current qml file, compared with another qml file (all file items, not just that folder).

str sFolder="\System"
str sFile="Q:\app - new, VS2015, broken\system.qml"

out

ARRAY(QMITEMIDLEVEL) a
if(!GetQmItemsInFolder(sFolder a)) end "no folder"

Sqlite& xnew=_qmfile.SqliteBegin(a[0].id)
Sqlite xold.Open(sFile 1)

int i iid rowid
for i 0 a.len
	iid=a[i].id
	GUID guid
	_qmfile.SqliteItemProp(+iid rowid guid)
	 xnew




_qmfile.SqliteEnd
