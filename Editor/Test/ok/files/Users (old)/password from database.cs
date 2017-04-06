 change these values
str user="username"
str database="$personal$\db1.mdb"
str table="tablename"
str userfield="userfieldname"
str passwordfield="passwordfieldname"
int debug=1 ;;change to 0 when retrieving password works well.

Database db
str connstr=db.CsAccess(database) ;;change this if using not Access database. To read more, click "Database" in the above line and press F1.
if(debug) out connstr
db.Open(connstr)
str sql.format("SELECT %s FROM %s WHERE %s='%s'" passwordfield table userfield user)
if(debug) out sql
ARRAY(str) a
db.QueryArr(sql a 1)
str password=a[0]

if(debug)
	out password
else
	AutoPassword user password 1|2 ;;note: will give error if there is no password field in the active window
