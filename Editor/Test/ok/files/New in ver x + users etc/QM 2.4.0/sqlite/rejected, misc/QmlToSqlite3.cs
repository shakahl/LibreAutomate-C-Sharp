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

Sqlite x.Open(db3File)

x.Exec("BEGIN TRANSACTION") ;;don't save until END TRANSACTION. Makes faster whe calling Exec multiple times.
if oneTable
	x.Exec("CREATE TABLE items (id INTEGER PRIMARY KEY,name TEXT,triggerEtc TEXT,flagsEtc TEXT,folder INTEGER,text TEXT)")
else
	x.Exec("CREATE TABLE items (id INTEGER PRIMARY KEY,name TEXT,triggerEtc TEXT,flagsEtc TEXT,folder INTEGER)")

 items
lpstr name triggerEtc flagsEtc folder text
str _name _triggerEtc _flagsEtc _text
int i
ARRAY(str) at
for i 0 2000000000
	j+4; if(j=s.len) break
	name=s+j
	triggerEtc=name+len(name)+1 ;;can contain not only trigger
	flagsEtc=triggerEtc+len(triggerEtc)+1 ;;flags[ date[ image]]
	folder=flagsEtc+len(flagsEtc)+1
	text=strstr(folder "[]"); if(!text) goto ge
	j=findb(s sep 4 text-s+2); if(j<0) goto ge
	text[0]=0
	
	_name=name; _name.SqlEscape
	_triggerEtc=triggerEtc; _triggerEtc.SqlEscape
	_flagsEtc=flagsEtc; _flagsEtc.SqlEscape
	_text=text+2; if(compress) _text.encrypt(32); else _text.SqlEscape
	if oneTable
		x.Exec(F"INSERT INTO items VALUES ({i},'{_name}','{_triggerEtc}','{_flagsEtc}',{folder},'{_text}')")
		 err int numErrors; numErrors+1
	else
		x.Exec(F"INSERT INTO items VALUES ({i},'{_name}','{_triggerEtc}','{_flagsEtc}',{folder})")
		at[]=_text

if !oneTable
	x.Exec("CREATE TABLE texts (id INTEGER PRIMARY KEY,text TEXT)")
	for i 0 at.len
		x.Exec(F"INSERT INTO texts VALUES ({i},'{at[i]}')")

x.Exec("END TRANSACTION") ;;save now
 x.Exec("VACUUM") ;;does not reduce file fragmentation
 out numErrors

err+ end _error
ret
 ge
end "bad file format"
