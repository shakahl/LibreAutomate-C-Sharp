 VbsExec2 "WScript.Echo 1"
WshExec "WScript.Echo 1"
 WshExec "WScript.Echo WScript.Arguments.Item(1)" _s "arg0 arg1"; out _s
 WshExec "WScript.Echo WScript.Arguments.Item(1)" 0 "arg0 arg1" "//T:2"
 WshExec "WScript.Echo(''bbb'');" 0 "" "//E:JScript"
 out "---"

#ret
out
int i
for i 0 3
	 WshExec F"WScript.Echo {i}" _s; out _s
	mac "WSH_thread" F"{i}"
	 0.2
