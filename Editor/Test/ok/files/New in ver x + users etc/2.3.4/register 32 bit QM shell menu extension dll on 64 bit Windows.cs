if(QMVER>=0x2030400) end "don't need this for this QM version"
if(!_win64) end "this is for 64-bit Windows"
if(!IsUserAdmin) end "QM must be admin"

str sf.format("$qm$\ver 0x%x\qmshex32.dll" QMVER)
sf.expandpath
cop "$qm$\qmshlex.dll" sf

str sk="CLSID\{C00E2DB5-3AF8-45a6-98CB-73FCDE00AC5B}\InprocServer32"
rset sf "" sk HKEY_CLASSES_ROOT
rset "Apartment" "ThreadingModel" sk HKEY_CLASSES_ROOT
 info: in this key is registered the 64-bit dll, but when we call rset from a 32-bit program, Windows redirects to the 32-bit CLSID key version.
