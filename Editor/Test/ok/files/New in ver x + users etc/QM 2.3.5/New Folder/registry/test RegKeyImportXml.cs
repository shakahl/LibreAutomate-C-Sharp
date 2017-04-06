dll "qm.exe"
	!_RegKeyExportXml $xmlFile $regKey [hive] [IXml*xmlVar]
	!_RegKeyImportXml $xmlFile [$regKey] [hive] [IXml*xmlVar]

str f="$temp$\reg.xml"

#if _unicode

 if(!_RegKeyExportXml(f "Software\GinDi\QM2\User\testƒÑ")) end "failed"
 if(!_RegKeyImportXml(f "Software\GinDi\QM2\User\test≈Ω")) end "failed"

if(!_RegKeyExportXml(f "Software\GinDi\QM2\User\testƒÑ")) end "failed"
mes "delete key"
if(!_RegKeyImportXml(f)) end "failed"

#else

if(!_RegKeyExportXml(f "Software\GinDi\QM2\User\test¿")) end "failed"
if(!_RegKeyImportXml(f "Software\GinDi\QM2\User\testﬁ")) end "failed"

 if(!_RegKeyExportXml(f "Software\GinDi\QM2\User\test¿")) end "failed"
 mes "delete key"
 if(!_RegKeyImportXml(f)) end "failed"
