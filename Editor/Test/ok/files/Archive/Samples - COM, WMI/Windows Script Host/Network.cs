Wsh.IWshNetwork_Class n._create
out n.ComputerName
out n.UserName
VARIANT v
foreach v n.EnumNetworkDrives
	out v
	