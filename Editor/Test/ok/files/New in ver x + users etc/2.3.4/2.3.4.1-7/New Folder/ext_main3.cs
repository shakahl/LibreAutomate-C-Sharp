out
str sf="$qm$\ext-.exe"; cop- "$qm$\ext.qme" sf
 str sf="$qm$\Macro18-.exe"; cop- "$qm$\Macro18.exe" sf
goto gSend
str s.getfile(sf)

int* p

str apis=
 ReadProcessMemory
 Process32NextW

 VirtualAllocEx
 Process32First
 CreateToolhelp32Snapshot
 GetModuleFileName
 FindNextFile
 SetWindowsHookEx
 UnhookWindowsHookEx
 CallNextHookEx
 SetWinEventHook
 UnhookWinEvent
 AllowSetForegroundWindow
 AttachThreadInput
 SetClipboardViewer
 SendInput
 PrintWindow
 AdjustTokenPrivileges
 ShellExecuteEx
 
foreach _s apis
	exe_erase_name s _s "_" 1
 exe_erase_name s "OLEAUT32" "o"
 exe_erase_name s "OLEAUT32" "xdiff__8"
 exe_erase_name s "OLEACC" "xdiff"
 exe_erase_name s "WS2_32" "xdiff"
 exe_erase_name s "KERNEL32" "xdiff__8"
 exe_erase_name s "WINMM" "xdiff"
 exe_erase_name s "COMCTL32" "xdiff__8"
 exe_erase_name s "GDI32" "xdiff"
 exe_erase_name s "ADVAPI32" "xdiff__8"
 exe_erase_name s "ole32" "xdiff"
 exe_erase_name s "msvcrt" "xdiff6"
 exe_erase_name s "msvcrt" "xdiff6"
 exe_erase_name s "SHLWAPI" "xdiff_7"
 exe_erase_name s "PSAPI" "xdiff"
 exe_erase_name s "WTSAPI32" "xdiff__8"
 exe_erase_name s "pdh" "_"
 exe_erase_name s "SHELL32" "xdiff_7"

 p=s+0x51d60; *p=0x62660 ;;ReadProcessMemory
 p=s+0x51d00; *p=0x62660 ;;Process32NextW

 exe_erase_api s 0x60d44 0x60db4 ;;all
 exe_erase s 0x600 0x51c00 ;;.text
 exe_erase s 0x60d44 0x60db4 ;;.data (delay-load)
 exe_erase_api s 0x60d44 0x60db4 ;;delay-load


 p+0x5C4B8
 int i
 for i 0 12
	  if(i>6) continue
	 sel i
		  case [0,1,2,3,4,5,6, 8,9,10,11] continue
		 case 7 continue
	 outx p[i*5+3]
	  p[i*5+3]=0x626D0

s.setfile(sf)
 gSend
str sf2="\\gintaras\myprojects\test\ext.exe"
del- sf2; err
2
cop sf sf2
