 Tested only on Win10.

if(RunConsole2("taskkill.exe /IM explorer.exe /F" _s)) out _s
1 ;;cannot delete files without waiting
ARRAY(str) a
str folder=iif(_winver<0x602 "$Local AppData$" "$local appdata$\Microsoft\Windows\Explorer")
GetFilesInFolder a folder "iconcache*.db";; out a
int i
for i 0 a.len
	FileDelete a[i]; err out F"{_error.description}. Close programs that may use it. Try to run this macro again.[][9]{a[i]}"
run "$windows$\explorer.exe"
