 this does not work

Database db.Open("DRIVER={MySQL ODBC 5.1 Driver};Server=www.quickmacros.com;Port=3306;Option=131072;USER=quickmac;PASSWORD=x;")
ARRAY(str) a; int r c
db.QueryArr("SELECT * FROM pet" a)
for r 0 a.len(2)
	out "-- Record %i --" r+1
	for c 0 a.len(1)
		out a[c r]

 "Driver={MySQL ODBC 5.1 Driver};" & _ 
           "Server=db1.database.com;" & _
           "Port=3306;" & _
           "Option=131072;" & _
           "Stmt=;" & _
           "Database=mydb;" & _
           "Uid=myUsername;" & _
           "Pwd=myPassword"