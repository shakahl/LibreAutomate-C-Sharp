function'str $dbFile [flags] [$dbPassword] [$systemDb] [$moreParams] ;;flags: 0 ODBC driver, 1 Jet/ACE, 2 Office 2007 format

 Creates connection string for MS Access.

 dbFile - Access database file (.mdb, .accdb).
 flags - what provider to use:
   0 (default) - Microsoft Access ODBC Driver.
   1 - Microsoft Jet OLE DB 4.0 or ACE OLEDB 12.0, depending on file format.
   2 (QM 2.3.4) - Office 2007 file format. This flag is optional if dbFile ends with ".accdb".
 dbPassword - database password. Can be encrypted (Options -> Security) for function Database.Open.
 systemDb - mdw file (if using a workgroup). Pass user id and password to the Open function.
 moreParams - will be appended to the connection string.

 REMARKS
 QM 2.3.4. Supports file format used by Office 2007 and later (.accdb). If dbFile ends with ".accdb" or used flag 2, creates connection string for .accdb. If flag 1, uses ACE OLEDB 12.0, else accdb driver.
 Else creates connection string for older file format (.mdb). If flag 1, uses Jet OLE DB 4.0, else driver.


str f1 f2 f3
int accdb=flags&2 or matchw(dbFile "*.accdb" 1)

if flags&1
	if(accdb) f1="Provider=Microsoft.ACE.OLEDB.12.0;Data Source=%s;"; if(empty(dbPassword)) f1+"Persist Security Info=False;"
	else f1="Provider=Microsoft.Jet.OLEDB.4.0;Data Source=%s;"
	f2="Jet OLEDB:System Database=%s;"
	f3="Jet OLEDB:Database Password=%s;"
else
	if(accdb) f1="Driver={Microsoft Access Driver (*.mdb, *.accdb)};Dbq=%s;"
	else f1="Driver={Microsoft Access Driver (*.mdb)};Dbq=%s;"
	f2="SystemDB=%s;"
	f3="pwd=%s;"

str s.format(f1 _s.expandpath(dbFile))
if(!empty(systemDb)) s.formata(f2 _s.expandpath(systemDb))
if(!empty(dbPassword)) s.formata(f3 dbPassword)
s+moreParams

ret s
