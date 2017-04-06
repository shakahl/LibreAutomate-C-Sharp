function $manifestFile [flags] ;;flags: 1 use caching

 Allows this thread to use COM components that are not registered in registry.
 Error if fails.

 manifestFile - file containing component info in XML format.
   Can be:
     1. Manifest file. You can create it with function <help>__ComActivator_CreateManifest</help>.
     2. Component file (dll etc) containing manifest resource. Use resource id, like "file.dll,2"; usually it is 1 or 2.
   Must be in qm folder. Can be full path or filename.
 flags:
   1 - use caching (in memory) to make faster. Don't use this flag when editing/debugging the manifest.

 REMARKS
 For .NET components, must be installed .NET framework runtime version that the component can use.
 More info in class help.


lock
ARRAY(__ACTCTXMAP)+ ___actCtxMap
int ac i
for i 0 ___actCtxMap.len
	if ___actCtxMap[i].manifest~manifestFile
		if(flags&1) ac=___actCtxMap[i].ac
		else ___actCtxMap.remove(i)
		break
if !ac
	__ACTCTXMAP& r=___actCtxMap[]
	r.ac.Create(manifestFile); err ___actCtxMap.redim(___actCtxMap.ubound); end _error
	r.manifest=manifestFile
	ac=r.ac

Deactivate
if(!ActivateActCtx(ac &m_cookie)) end "" 16
