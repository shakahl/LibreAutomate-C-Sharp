 /exe
 out GetModuleHandle("sqlite3.dll")
str file.expandpath("$desktop$\test.db3")
FileExists(file)
 del- file; err
PF
Sqlite x.Open(file)
PN
ARRAY(str) a
rep 5
	x.Exec("select * from highscores order by score desc" a)
	PN
PO
out a
 out GetModuleHandle("sqlite3.dll")

 BEGIN PROJECT
 main_function  Macro251
 exe_file  $my qm$\Macro251.qmm
 flags  6
 guid  {EBC60985-80F5-4E4B-A537-F0691B5F60EB}
 END PROJECT
