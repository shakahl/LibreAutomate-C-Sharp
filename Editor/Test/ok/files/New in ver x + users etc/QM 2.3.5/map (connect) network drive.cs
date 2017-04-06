 Note: there are separate mappings for standard user and admin. If QM is admin, the mapping is not in Windows Explorer, and is not visible to non-admin programs.

Wsh.WshNetwork n._create
VARIANT v=1
n.MapNetworkDrive("N:" "\\gintaras\myprojects\app" v)

out dir("N:\qm.exe")
10

 Wsh.WshNetwork n._create
 VARIANT v=1
n.RemoveNetworkDrive("N:" v v)
