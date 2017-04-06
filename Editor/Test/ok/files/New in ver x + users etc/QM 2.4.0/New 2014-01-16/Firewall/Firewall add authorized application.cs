str appName="Atest"
str appPath.expandpath("$my qm$\Atest.exe")

typelib NetFwTypeLib {58FBCF7C-E7A9-467C-80B3-FC65E8FCCA08} 1.0

NetFwTypeLib.INetFwMgr manager._create("{304CE942-6E39-40D8-943A-B913C40C9CD4}")
NetFwTypeLib.INetFwAuthorizedApplications apps=manager.LocalPolicy.CurrentProfile.AuthorizedApplications
NetFwTypeLib.INetFwAuthorizedApplication a

int i=list("Allow[]Block[]Find[]Remove" appPath "Firewall")
sel i
	case [1,2] ;;add
	a._create("HNetCfg.FwAuthorizedApplication")
	a.Name=appName
	a.ProcessImageFileName=appPath
	a.Enabled=iif(i=1 TRUE 0)
	apps.Add(a)
	
	case 3 ;;find
	int isInList
	foreach(a apps) if(appPath~_s.from(a.ProcessImageFileName)) isInList=1; break
	mes F"{iif(isInList `found` `not found`)}[]{appPath}"
	
	case 4 ;;remove
	apps.Remove(appPath)
