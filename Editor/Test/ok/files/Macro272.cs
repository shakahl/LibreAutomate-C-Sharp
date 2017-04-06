if !FileExists("H:" 2)
	Wsh.WshNetwork n._create
	VARIANT v=1
	n.MapNetworkDrive("H:" "\\xxx\yyy\zzz" v)
	n.MapNetworkDrive("P:" "\\xxx\yyy\zzz2" v)
