out
str friendID="294756645"
str url.format("http://collect.myspace.com/index.cfm?fuseaction=bandprofile.listAllShows&friendid=%s" friendID)
HtmlDoc d.InitFromWeb(url)
str s=d.GetHtml
 out s
 d.GetForm(1 0 _s); out _s ;;debug

str dbfile="$desktop$\Shows.db3"
str sql
Sqlite db1.Open(dbfile)
db1.Exec("BEGIN TRANSACTION") ;;makes faster and safer
db1.Exec("CREATE TABLE IF NOT EXISTS Shows (Band,DateTime,Venue,Address,City,Zip_Code,State,Cost,Description)")

ARRAY(POSTFIELD) a
int i
for i 1 1000000 ;;don't know how many forms
	d.GetForm(i 0 0 a); err break
	 out a[0].name
	if(a[0].name~"calEvtLocation"=0) break
	
	 str _ss; d.GetForm(i 0 _ss); out _ss; out "-----------------------------------"; continue ;;debug
	
	str sBand calEvtDateTime calEvtLocation calEvtStreet calEvtCity calEvtZip calEvtState sCost sDescription
	 sBand=a[0].value
	 calEvtDateTime=a[6].value
	 ...
	
	 NOTE: some fields of some shows are missing. For example street.
	 Then we cannot simply use calEvtDateTime=a[6].value. Need to find a[?].name.
	
	int j
	str* sp=&sBand
	for(j 0 9) sp[j].all ;;clear all variables
	
	for(j 0 a.len) if(a[j].name="calEvtLocation") sBand=a[j].value; break
	for(j 0 a.len) if(a[j].name="calEvtDateTime") calEvtDateTime=a[j].value; break
	 ...
	
	 before passing strings to sqlite, necessary to escape ' to ''
	for(j 0 9) sp[j].findreplace("'" "[39]'")
	
	 delete old row(s) where band and date fields match new band and date
	db1.Exec(sql.format("DELETE FROM Shows WHERE Band='%s' AND DateTime='%s'" sBand calEvtDateTime))
	
	sql.format("INSERT INTO Shows VALUES ('%s','%s','%s','%s','%s','%s','%s','%s','%s')" sBand calEvtDateTime calEvtLocation calEvtStreet calEvtCity calEvtZip calEvtState sCost sDescription)
	db1.Exec(sql)

db1.Exec("END TRANSACTION")

err+
	QMERROR e=_error; e.description.formata("[][]Last SQL statement: %s" sql)
	db1.Exec("ROLLBACK TRANSACTION"); err
	end e
