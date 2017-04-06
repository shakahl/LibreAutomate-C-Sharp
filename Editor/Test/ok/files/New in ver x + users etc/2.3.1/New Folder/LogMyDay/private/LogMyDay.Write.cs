 /LogMyDay help

if(m_flags&1) out "time span: %s, %i s" m_s m_ns

str sql s st; int i h m
ARRAY(str) a

 create table
GetTodaysTableName(s) ;;auto create table name from today's date, so that each day's activity would be in separate table
if(s!=m_table)
	m_table=s
	sql.format("CREATE TABLE %s (Program TEXT, Time_total TEXT, Time_s INTEGER)" m_table)
	m_db.Query(sql); err

 try to get existing record for the program
sql.format("SELECT * FROM %s WHERE Program='%s'" m_table m_s)
m_db.QueryArr(sql a 1)

if a.len ;;already exists. Update.
	i=val(a[2])+m_ns
	if(m_flags&1) out "total time: %s, %i s" a[0] i
	FormatTime(i st)
	sql.format("UPDATE %s SET Time_total='%s', Time_s=%i WHERE Program='%s'" m_table st i m_s)
	m_db.Query(sql)
else ;;add new
	FormatTime(m_ns st)
	sql.format("INSERT INTO %s VALUES('%s','%s',%i)" m_table m_s st m_ns)
	m_db.Query(sql)

err+ end _error
