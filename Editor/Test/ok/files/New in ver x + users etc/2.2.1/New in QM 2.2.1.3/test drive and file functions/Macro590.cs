Wsh.Drive d=GetDrive("a")
if(d.IsReady)
	double asp=d.AvailableSpace
	out asp/1024/1024 ;;MB
else out "not ready"
