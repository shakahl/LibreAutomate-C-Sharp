 /exe
if(EXE) 5
 #ret
Sqlite x.Open("$my qm$\test\ok.db3")
 Sqlite x.Open("$my qm$\test\main.db3")
if(EXE) 3
ARRAY(str) a
 x.Exec("select name from items" a)
 x.Exec("select * from items" a)
x.Exec("select text from items WHERE name=='Macro4'" a)
out a.len
if(EXE) 3
a=0
if(EXE) 3

 BEGIN PROJECT
 main_function  Macro2055
 exe_file  $my qm$\Macro2055.qmm
 flags  6
 guid  {20A20B07-0561-4970-BF9C-E1AB15FF6534}
 END PROJECT
