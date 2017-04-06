 /
function $dllFile [flags] ;;flags: 1 unregister, 2 create type library (.tlb file)

 Registers or unregisters .NET dll as COM component.
 Error on failure.
 QM must be running as administrator.

 REMARKS
 It is possible to use .NET COM components without registration. Use __ComActivator class.

 EXAMPLE
 RegisterNetComComponent "$desktop$\ClassLibrary2.dll"


str cl so
if(!GetNetRuntimeFolder(cl)) end ES_FAILED

 cl.formata("\regasm.exe ''%s'' /nologo /codebase" _s.expandpath(dllFile))
cl.formata("\regasm.exe ''%s'' /nologo" _s.expandpath(dllFile))
if(flags&1) cl+" /u"
else if(flags&2) cl+" /tlb"

int ec=RunConsole2(cl so)
err end _error
so.trim
if(ec) end so
if(so.len) out so
