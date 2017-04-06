function $manifestFile

 Creates COM component activation context.
 Usually you don't use this class directly. Instead use __ComActivator.


Clear
str sf.expandpath(manifestFile)
if !sf.begi(_qmdir)
	if(PathIsRelative(sf)) sf-_qmdir
	else end "the file must be in qm folder"
int resId=PathParseIconLocation(sf)
if(!FileExists(sf)) end "file not found"

ACTCTXW ac.cbSize=sizeof(ac)
ac.lpSource=@sf
if(resId) ac.dwFlags|ACTCTX_FLAG_RESOURCE_NAME_VALID; ac.lpResourceName=+resId

_i=CreateActCtxW(&ac); if(_i=-1) end "" 16
m_hctx=_i

 Notes:
 Fails on XP SP0. Why? Manifest must be embedded? Not tested on SP1. MSDN says COM supported on XP, .NET on XP SP2.
 Tried to make faster: call GetModuleHandleW and use ac.hModule. If module loaded, faster 30%, but if not, slower 2 times. Why GetModuleHandleW so slow?.
 Tried lpAssemblyDirectory, manifest etc to make it work when assembly file is not in app folder, unsuccessfully.
