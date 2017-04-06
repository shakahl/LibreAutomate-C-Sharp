int i=list("Remove regcode[]Remove regcode and evaluation data")
sel i
	case 0 ret
	case 2
	rset 0 "Syncmgr" "Software\Microsoft\Windows\CurrentVersion\Explorer" 0 -2
	FileDelete "$temp$:Session.Id"; err
	 rset 0 "" "CLSID\{C4B11A95-B842-2E07-02F9-47C1F27A3D62}\InProcServer32" HKEY_CLASSES_ROOT -1 ;;old version
rset 0 "QMX" "Software\GinDi" 0 -1
rset 0 "QMX" "Software\GinDi" HKEY_LOCAL_MACHINE -1
