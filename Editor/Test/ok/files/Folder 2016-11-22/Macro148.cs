out
str cl=
 q:\app\catkeys\tasks\csc.exe
 /nologo
 /noconfig
 /out:C:\Users\G\AppData\Local\Catkeys\ScriptDll\test.exe
 /target:winexe
 /r:Q:\app\Catkeys\Tasks\Catkeys.dll
 Q:\Test\test.cs

cl.findreplace("[]" " ")
 out cl;ret
out RunConsole2(cl)
