 Compares current System.qml with an old System.qml (oldSystem) and shows all functions that are in old but not in new.
 Adds the old functions to remSystem. Does not remove/replace existing functions in remSystem. Auto-creates remSystem if does not exist.
 Not finished. Now using macro "Removed System functions" instead.

str oldSystem="$qm$\System - before using sub in System.QML"
str remSystem="$qm$\Removed System private functions.QML"
if(!dir(remSystem)) sub.CreateRemSystem remSystem
 _______________________________________________________

out
Sqlite& xnew=_qmfile.SqliteBegin(qmitem("init"))
Sqlite xold.Open(oldSystem)
Sqlite xrem.Open(remSystem)
xrem.Exec("BEGIN")

ARRAY(str) ao an as at
xold.Exec("SELECT name,id FROM items WHERE flags&255=2" ao)
xnew.Exec("SELECT name FROM items WHERE flags&255=2" an)
int i j n
for i 0 ao.len ;;for each old function
	 is it in new file?
	lpstr fname=ao[0 i]
	for(j 0 an.len) if(an[0 j]=fname) break
	if(j<an.len) continue ;;yes it is
	out fname
	n+1
	 add to remSystem
	xrem.Exec(F"SELECT 1 FROM items WHERE name='{fname}'" as); if(as.len) continue
	xrem.Exec(F"INSERT INTO items(pid,flags,name) VALUES(0,16777218,'{fname}')") ;;flags 16777218 (0x1000002): function, has text
	xold.Exec(F"SELECT text FROM texts WHERE id={ao[1 i]}" at)
	str& st=at[0 0]; st.SqlEscape
	xrem.Exec(F"INSERT INTO texts(text) VALUES('{st}')")
	
out n

xrem.Exec("COMMIT")
_qmfile.SqliteEnd()


#sub CreateRemSystem
function $remSystem
str st=
 These functions have been moved from System.qml to this file.
 They are no longer used by QM, but maybe used in your macros, for example in cloned old System functions.

int iid=newitem("About 'Removed System private functions'" st "" "" "" 128)
SilentExport +iid remSystem 2
