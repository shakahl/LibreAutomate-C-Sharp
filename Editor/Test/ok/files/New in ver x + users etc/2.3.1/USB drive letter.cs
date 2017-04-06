 add this somewhere at the beginning
str+ g_mydrive.left(_qmdir 2)
SetEnvVar "mydrive" g_mydrive

 then anywhere in your macro you can use g_mydrive. Example:
out g_mydrive

 also, anywhere in your macro with QM file functions you can use %mydrive% as your drive name. Example:
run "%mydrive%\folder\some.exe"
