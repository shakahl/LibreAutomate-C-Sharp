function $manifestFile

 Creates COM component activation context.
 Usually you don't use this class directly. Instead use __ComActivator.


if(_winver<0x501) end "bad OS version"

Clear
str sf.expandpath(manifestFile) sdir
if !sf.begi(_qmdir)
	if(PathIsRelative(sf)) sf-_qmdir
	else end "the file must be in qm folder"
int resId=PathParseIconLocation(sf)
 if(!dir(sf)) end "file not found"
sf="q:\app\csscriptlibrary.dll"
sf="C:\Users\G\AppData\Local\Temp\CSSCRIPT\CSScriptLibrary.dll"
out dir(sf)

ACTCTXW ac.cbSize=sizeof(ac)
ac.lpSource=@sf
if(resId) ac.dwFlags|ACTCTX_FLAG_RESOURCE_NAME_VALID; ac.lpResourceName=+resId
ac.lpAssemblyDirectory=@sdir.getpath(sf ""); ac.dwFlags|ACTCTX_FLAG_ASSEMBLY_DIRECTORY_VALID
out sdir
out resId
 ac.lpApplicationName="CsScriptLibrary"; ac.dwFlags|ACTCTX_FLAG_APPLICATION_NAME_VALID

_i=CreateActCtxW(&ac); out _i; if(_i=-1) end _s.dllerror
m_hctx=_i

 note: fails on XP SP0. Why? Manifest must be embedded? Not tested on SP1. MSDN says COM supported on XP, .NET on XP SP2.
 The functions unavailable on win2000.
 Tried to make faster: call GetModuleHandleW and use ac.hModule. If module loaded, faster 30%, but if not, slower 2 times. Why GetModuleHandleW so slow?.
