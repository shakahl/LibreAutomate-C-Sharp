typelib NetFwTypeLib {58FBCF7C-E7A9-467C-80B3-FC65E8FCCA08} 1.0
NetFwTypeLib.INetFwPolicy2 f._create("HNetCfg.FwPolicy2")
str name="Atest"
int add=0
if add
	NetFwTypeLib.INetFwRule r._create("HNetCfg.FWRule")
	r.Name=name
	r.Action=NetFwTypeLib.NET_FW_ACTION_ALLOW
	r.Description="Allow Atest"
	r.ApplicationName=_s.expandpath("$my qm$\Atest.exe")
	r.Enabled=TRUE
	r.InterfaceTypes="All"
	f.Rules.Add(r)
else
	f.Rules.Remove(name)
