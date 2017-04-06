/exe

 Don't run directly from app, because then no uiAccess, even if with 0x10000 (consent).

int test=!EXE
str f1 f2 sCopied sFailed
ARRAY(COMPAREFIF) a; int i

CompareFilesInFolders a iif(test "$qm$" "\\vmware-host\Shared Folders\Q\app") "$pf$\quick macros 2" "*.exe[]*.dll"
for i 0 a.len
	COMPAREFIF& x=a[i]; if(x.flags&1=0) continue
	_s=F"'{x.f1}'  '{x.f2}'"
	if(!test) FileCopy x.f1 x.f2; err sFailed.addline(F"{_s}[]   {_error.description}"); continue
	sCopied.addline(_s)

CompareFilesInFolders a "$pf$\quick macros 2" F"$pf$\quick macros 2\ver 0x{QMVER}" "*.dll"
for i 0 a.len
	&x=a[i]; if(x.flags&1=0) continue
	_s=F"'{x.f1}'  '{x.f2}'"
	if(!test) FileCopy x.f1 x.f2; err sFailed.addline(F"{_s}[]   {_error.description}"); continue
	sCopied.addline(_s)

if(!test) run "$pf$\quick macros 2\qm.exe" "v"

if(!sCopied.len and !sFailed.len) ret
if(test) out sCopied
else
	if(sCopied.len) OnScreenDisplay F"COPIED[]{sCopied}" 10 -1 0 "" 8 0xff0000 7
	if(sFailed.len) OnScreenDisplay F"FAILED[]{sFailed}" 10 -1 0 "" 8 0xff 7

 BEGIN PROJECT
 main_function  QM app to PF
 exe_file  $desktop$\QM app to PF.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  4
 guid  {DEAA31FE-9A2C-4F1E-B7D3-C4B4D6DB9558}
 END PROJECT
