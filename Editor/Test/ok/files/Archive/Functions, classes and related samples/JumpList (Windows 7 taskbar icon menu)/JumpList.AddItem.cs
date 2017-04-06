 /JumpList help
function $name [$path] [$arguments] [$icon] [$description]

 Adds an item to the jump list.

 name - display name.
   To add separator, use "-". Then other parameters not used.
 path - path of program to run on click.
   If empty, uses path of current process (QM or exe).
   Also can be a document.
 arguments - command line arguments to pass to the program.
 icon - path of icon file.
   If empty, uses icon of program. However cannot get icon of a document or other file that does not have icons itself.
   Can contain icon index or resource id in standard format: "file,index", "file,-resID".
 description - tooltip text.


#opt nowarnings 1
IShellLinkW x._create(CLSID_ShellLink)
IPropertyStore ps=+x
PROPERTYKEY pk
VARIANT v

if !StrCompare(name "-")
	pk.pid=6; memcpy &pk.fmtid uuidof("{9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3}") sizeof(GUID)
	v.vt=VT_BOOL; v.boolVal=-1
else
	pk.pid=2; memcpy &pk.fmtid uuidof("{F29F85E0-4FF9-1068-AB91-08002B27B3D9}") sizeof(GUID)
	v=name
	
	str sp
	if(empty(path)) sp=ExeFullPath; else if(!sp.searchpath(path)) end ERR_FILE
	x.SetPath(@sp)
	
	if(!empty(arguments)) x.SetArguments(@arguments)
	
	int ii
	if(empty(icon)) icon=sp; else icon=_s.expandpath(icon); ii=PathParseIconLocation(_s)
	x.SetIconLocation(@icon ii)
	
	if(!empty(description)) x.SetDescription(@description)

ps.SetValue(&pk +&v)
ps.Commit

m_col.AddObject(x)

err+ end _error

 Tested: user cannot remove these items, therefore we can ignore removed items. Maybe can remove only added through IShellItem?
