Wsh.Drive d=GetDrive("c")
if(d.IsReady)
	double asp=d.AvailableSpace
	out asp/1024/1024 ;;MB
	 outref d
else out "not ready"
