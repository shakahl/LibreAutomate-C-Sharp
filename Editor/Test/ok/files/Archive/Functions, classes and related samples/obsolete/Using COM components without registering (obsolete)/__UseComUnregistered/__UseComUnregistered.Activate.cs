function $manifestFile [resourceId]

 Enables creation of classes of unregistered COM component.
 Error if fails.

 manifestFile - manifest file created by __UseComUnregistered_CreateManifest.
   Or can be exe or dll file containing manifest resource. Then use resourceId.
   Must be in qm folder. Can be full path or filename.
 resourceId - manifest resource id in exe or dll file.
 flags:
   1 - try to get handle of dll containing manifest. If not loaded, get from file.

 The dll file must be in qm folder or its subfolder, as when creating the manifest.


if(_winver<0x501) end "bad OS version"

Clear
ACTCTXW ac.cbSize=sizeof(ac)
if(!_s.searchpath(manifestFile "$qm$")) end "file not found"
if(!_s.begi(_qmdir)) end "the file must be in qm folder"
ac.lpSource=@_s
if(resourceId) ac.dwFlags|ACTCTX_FLAG_RESOURCE_NAME_VALID; ac.lpResourceName=+resourceId

_i=CreateActCtxW(&ac); if(_i=-1) end _s.dllerror
m_hctx=_i
if(!ActivateActCtx(m_hctx &m_cookie)) end _s.dllerror

 note: CreateActCtxW says "incorrect parameter" on XP SP0. Why? Manifest must be embedded?
 The functions unavailable on win2000.
 Tried to make faster: call GetModuleHandleW and use ac.hModule. If module loaded, faster 30%, but if not, slower 2 times. Why GetModuleHandleW so slow?.
