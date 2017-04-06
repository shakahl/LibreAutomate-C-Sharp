 Compares current System.qml with an old System.qml (oldSystem) and shows all functions that are in old but not in new.
 Adds the old functions to table xRemoved in current System.qml. Does not remove/replace existing functions in the table.

 In the future, when a private System function removed, just run this macro to update xRemoved table. Don't need to change oldSystem.
 Most private System functions removed in QM 2.4.1. They replaced with sub-functions. The oldSystem file is System.qml before that.
 Don't need to run this macro when removing functions added after QM 2.4.1. QM now shows warning when a private System function used in user code.

 str oldSystem="$qm$\System - before using sub in System.QML"
 str oldSystem="$qm$\System - 2.4.1.QML"
str oldSystem="$qm$\System - 2.4.2.QML"
 _______________________________________________________

out
 ICsv c._create; c.FromString(
Sqlite& xnew=_qmfile.SqliteBegin(qmitem("init"))
Sqlite xold.Open(oldSystem 2)
 xnew.Exec("DROP TABLE IF EXISTS xRemoved")
xnew.Exec("CREATE TABLE IF NOT EXISTS xRemoved(name TEXT UNIQUE,text TEXT)")

ARRAY(str) ao an as at
xold.Exec("SELECT name,id,flags FROM items" ao)
xnew.Exec("SELECT name,flags FROM items WHERE flags&255!=5" an)
int i j n
for i 0 ao.len ;;for each old function
	 is it in new file?
	lpstr fname=ao[0 i]
	int flags=val(ao[2 i])
	for(j 0 an.len) if(an[0 j]=fname and val(an[1 j])&255=flags&255) break
	if(j<an.len) continue ;;yes it is
	
	 skip some
	if(flags&255=6) out F"<><c 0x8080>FOLDER: {fname}</c>"; continue
	sel(fname 2)
		case ["DE_*","TO_Wait_*","EA_*","EH_*","TO_Acc*","TO_Htm*","EW_*","ILE_*","TO_HelpSection*","TO_RKK_*","TO_ECK_*","___HelpIndexer.*","CHI_*","__ToolsControl.*","__TC_*","LD_*","EOW_*","TO_Scan_*","TO_FileTest","TO_WebInfo","TO_Test*","Dialog*_s*"]
		out F"<><c 0x8000>SKIPPED: {fname}</c>"
		continue
		 case ["smtp_*","p_*","me99_*"] ;;COM events. Useless because must be in certain folder.
	if(flags&0x200) out F"<><c 0xff>ENCRYPTED: {fname}</c>"; continue
	sel(flags&255)
		case 2
		case 7 out F"<><c 0xff0000>MEMBER FUNCTION: {fname}</c>"; continue ;;currently there are no member functions that could be useful. Auto-adding not implemented.
		case else out F"<><c 0xff0000>NOT FUNCTION: {fname}</c>"; continue ;;currently there are no items of other types that could be useful
	
	 add to xRemoved
	xnew.Exec(F"SELECT 1 FROM xRemoved WHERE name='{fname}'" as); if(as.len) continue
	xold.Exec(F"SELECT text FROM texts WHERE id={ao[1 i]}" at)
	str& st=at[0 0]; st.SqlEscape
	
	 xnew.Exec(F"INSERT INTO xRemoved VALUES('{fname}','{st}')") ;;enable this to actually add
	
	out fname
	n+1
out n

 xnew.Exec("SELECT name FROM xRemoved" an)

_qmfile.SqliteEnd

 str sn
 for(i 0 an.len) sn.addline(an[0 i])
 sn.setmacro("xRemoved names")
