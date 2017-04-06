 Repairs the problem when net cannot send to XP.
 Particularly, disables/enables the network connection.
 Usually you'll need to run this macro only on Vista, but may also have to run on XP.

int h h2
Acc a

if _winnt>=6 ;;Vista
	if(_winver!=0x600) mes- "macro for this OS version unavailable"
	
	run "$49$" "" "" "" 0x800 win("Network Connections" "CabinetWClass") h ;;Network Connections
	
	a=acc("Local Area Connection 2" "LISTITEM" h "SysListView32" "" 0x1001 0 0 "" 10)
	a.Select(2)
	a=acc("Disable this network device" "PUSHBUTTON" h "DirectUIHWND" "" 0x1001 0 0 "" 10)
	a.DoDefaultAction
	
	 now manually close UAC consent
	
	5
	wait 30 WA h ;;waiting until UAC consent closed
	5
	
	a=acc("Diagnose this connection" "PUSHBUTTON" h "DirectUIHWND" "" 0x1001)
	a.DoDefaultAction
	
	h2=wait(0 WV win("Windows Network Diagnostics" "#32770"))
	a=acc("Enable the network adapter *" "PUSHBUTTON" h2 "Button" "" 0x1001 0 0 "" 10)
	a.DoDefaultAction
	
	 now manually close UAC consent and wait a while
	 then close 'Network Connections' window

else ;;XP
	run "$3$ 1E00718000000000000000000000C7AC07700232D111AAD200805FC1270E" "" "" "" 0x800 win("Network Connections" "ExploreWClass") h ;;Network Connections
	a=acc("Local Area Connection" "LISTITEM" h "SysListView32" "" 0x1001 0 0 "" 10)
	act child(a)
	a.Select(2)
	men 29201 win("Network Connections" "ExploreWClass") ;;Disable
	5
	men 29200 win("Network Connections" "ExploreWClass") ;;Enable
	
	 now wait a while and close 'Network Connections' window
