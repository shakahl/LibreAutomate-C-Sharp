str s title cls exe
ARRAY(int) handles
GetWindowList 0 "" 0 0 0 handles

for(int'i 0 handles.len)
	int hwnd=handles[i]
	if(!IsWindowVisible(hwnd)) continue ;;exclude hidden
	if(GetWinStyle(hwnd 1)&WS_EX_TOOLWINDOW) continue ;;exclude such windows as QM toolbars, taskbar, most other toolbars, most tooltips, etc
	exe.getwinexe(hwnd 1)
	title.getwintext(hwnd)
	cls.getwinclass(hwnd)
	if(exe.endi("blackbox.exe")) exe = "explorer.exe *1"
	if(cls=="HH Parent") exe = "c:\windows\hh.exe *0"
	title.findreplace(":" " ") ;;escape :
	title.escape(1) ;;escape "
	s.formata("%s :act %i; err * %s[]" title hwnd exe)

 out s

int htb=win("MY_TASKBAR" "QM_Toolbar")
if(htb) s.setmacro("my_taskbar") ;;just replace text, which will automatically update the toolbar
else htb=DynamicToolbar(s "my_taskbar")
