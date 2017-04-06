out
str s.expandpath("$my qm$\Macro1900.exe")
str so1 so2

 int fs1=GetFileOrFolderSize(s)/1024
Q &q
RunConsole2 F"upx.exe ''{s}''" so1
 int fs2=GetFileOrFolderSize(s)/1024
Q &qq
RunConsole2 F"upx.exe -d ''{s}''" so2
 int fs3=GetFileOrFolderSize(s)/1024
Q &qqq
outq
 out "%i %i %i" fs1 fs2 fs3
out so1
out so2
