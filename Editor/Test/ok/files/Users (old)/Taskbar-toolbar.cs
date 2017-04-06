str titles exe
ARRAY(int) handles
GetWindowList &titles "" 1|2|4 0 0 handles
ARRAY(str) arr = titles

for(int'i 0 arr.len)
	int hwnd=handles[i]
	exe.getwinexe(hwnd 1)
	if _s.getfilename(exe 0)=="blackbox"
		exe = "explorer.exe *1"
	if arr[i]=="QM Help"
		exe = "c:\windows\hh.exe *0"
	arr[i].findreplace(":" " ") ;;escape :
	arr[i].escape(1) ;;escape "
	arr[i].formata(" :act %i; err * %s" hwnd exe)
	
titles=arr

int htb=win("MY_TASKBAR" "QM_Toolbar")
if(htb) titles.setmacro("my_taskbar") ;;just replace text, which will automatically update the toolbar
else htb=DynamicToolbar(titles "my_taskbar")
