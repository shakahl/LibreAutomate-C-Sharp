typelib Wsh {F935DC20-1CF0-11D0-ADB9-00C04FD58A0B} 1.0

Wsh.WshShell shell._create
Wsh.WshEnvironment env=shell.Environment

VARIANT v
foreach v env._NewEnum FE_ComColl
	out v
