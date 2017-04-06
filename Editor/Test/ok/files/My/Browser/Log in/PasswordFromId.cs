 /
function# str'Kur str&Id str&password

Database d.Open(d.CsExcel("%doc%\id.xls"))
ARRAY(str) a
 g1
str tables("Web[]Upload") table
foreach table tables
	str sql.format("SELECT id,password FROM [%s$] WHERE Kur LIKE '%%%s%%'" table Kur)
	d.QueryArr(sql a 1)
	if(a.len) break
if(!a.len) mes- "not found"
Id=a[0]
password=a[1]
 out Id
 out password
if(password.beg("*"))
	if(password.beg("**"))
		if(!inp(_s "common 2")) ret
		password.replacerx("^\*\*" _s 4)
		lpstr m="svkbgblrrslg"
		password[4]=m[password[4]-'1']
	else
		if(!inp(_s "common 1")) ret
		password.replacerx("^\*" _s 4)
else if(matchw(password "<*>"))
	Kur.get(password 1 password.len-2)
	goto g1

ret 1
