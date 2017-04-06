 /test QmlToSqlite
function $qmlFile $db3File flags ;;flags: 0 text in separate table, 1 text in same table, 2 compress text

int oneTable=flags&1
int compress=flags&2

if(dir(db3File)) del- db3File

str s.getfile(qmlFile)
lpstr sep="[][0][0]"

 header
int k j=findb(s sep 4)
if(!s.beg("//QM v2.") or j<0) goto ge

Sqlite x.Open(db3File 0 2)
SqliteStatement p
 x.Exec(F"PRAGMA page_size={1024*4}") ;;if 64K, sqlite manager cannot open

x.Exec("BEGIN TRANSACTION") ;;don't save until END TRANSACTION. Makes faster whe calling Exec multiple times.
if oneTable
	 x.Exec("CREATE TABLE items (id INTEGER PRIMARY KEY,name TEXT,triggerEtc TEXT,flagsEtc TEXT,folder INTEGER,text TEXT)") ;;works the same
	x.Exec(F"CREATE TABLE items (id INTEGER PRIMARY KEY,name TEXT,triggerEtc TEXT,flagsEtc TEXT,folder INTEGER,text {iif(compress `BLOB` `TEXT`)})")
	p.Prepare(x "INSERT INTO items VALUES (?1,?2,?3,?4,?5,?6)")
else
	x.Exec("CREATE TABLE items (id INTEGER PRIMARY KEY,name TEXT,triggerEtc TEXT,flagsEtc TEXT,folder INTEGER)")
	p.Prepare(x "INSERT INTO items VALUES (?1,?2,?3,?4,?5)")

 items
lpstr name triggerEtc flagsEtc folder text
 str _name _triggerEtc _flagsEtc _text
int i compr
ARRAY(str) at
for i 0 2000000000
	j+4; if(j=s.len) break
	name=s+j
	triggerEtc=name+len(name)+1 ;;can contain not only trigger
	flagsEtc=triggerEtc+len(triggerEtc)+1 ;;flags[ date[ image]]
	folder=flagsEtc+len(flagsEtc)+1
	text=strstr(folder "[]"); if(!text) goto ge
	j=findb(s sep 4 text-s+2); if(j<0) goto ge
	text[0]=0; text+2; s[j]=0
	
	 _name=name; _name.SqlEscape
	 _triggerEtc=triggerEtc; _triggerEtc.SqlEscape
	 _flagsEtc=flagsEtc; _flagsEtc.SqlEscape
	p.Reset
	p.BindInt(1 i); p.BindText(2 name); p.BindText(3 triggerEtc); p.BindText(4 flagsEtc); p.BindText(5 folder)
	if oneTable
		compr=0; if(compress and text[0]) _s.encrypt(32 text); compr=_s.len<len(text)
		if(compr) p.BindBlob(6 _s _s.len); else p.BindText(6 text)
	else
		at[]=text
	p.Exec
	 err int numErrors; numErrors+1

if !oneTable
	 x.Exec("CREATE TABLE texts (id INTEGER PRIMARY KEY,text TEXT)") ;;works the same
	x.Exec(F"CREATE TABLE texts (id INTEGER PRIMARY KEY,text {iif(compress `BLOB` `TEXT`)})")
	p.Prepare(x "INSERT INTO texts VALUES (?1,?2)")
	for i 0 at.len
		str& r=at[i]
		if(!r.len) continue ;;eg skip folders
		p.Reset
		p.BindInt(1 i)
		compr=0; if(compress and r.len) _s.encrypt(32 r); compr=_s.len<r.len
		if(compr) p.BindBlob(2 _s _s.len); else p.BindText(2 r)
		p.Exec

x.Exec("ANALYZE")
x.Exec("END TRANSACTION") ;;save now
 x.Exec("VACUUM") ;;does not reduce file fragmentation
 out numErrors

 err+ end _error
ret
 ge
end "bad file format"
