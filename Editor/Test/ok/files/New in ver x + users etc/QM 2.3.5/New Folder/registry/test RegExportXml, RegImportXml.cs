out
dll "qm.exe" [RegExportXml]RegExportXml2 $xmlFile $regKey [hive]
dll "qm.exe" [RegImportXml]RegImportXml2 $xmlFile [$regKey] [hive]

str sk="Software\GinDi\QM2"
str sf.expandpath("$temp$\RegSaveKey.txt")
str sf2.expandpath("$temp$\RegSaveKey2.txt")

 EXPORT

PF
RegExportXml sf sk
PN
if(!RegExportXml2(sf2 sk)) end "failed"
PN; PO
 run sf
_s.getfile(sf); _s.getl(_s 1 2); out F"{GetFileOrFolderSize(sf)} {Crc32(_s _s.len)}" 
_s.getfile(sf2); _s.getl(_s 1 2); out F"{GetFileOrFolderSize(sf2)} {Crc32(_s _s.len)}" 

 IMPORT

sk="Software\GinDi Test"
rset 0 "1" sk 0 -2
rset 0 "2" sk 0 -2
PF
RegImportXml sf F"{sk}\1"
PN
if(!RegImportXml2(sf2 F"{sk}\2")) end "failed"
PN; PO

 test export imported
sf+".txt"; sf2+".txt"
RegExportXml sf F"{sk}\1"
if(!RegExportXml2(sf2 F"{sk}\2")) end "failed"
_s.getfile(sf); _s.getl(_s 1 2); out F"{GetFileOrFolderSize(sf)} {Crc32(_s _s.len)}" 
_s.getfile(sf2); _s.getl(_s 1 2); out F"{GetFileOrFolderSize(sf2)} {Crc32(_s _s.len)}" 
run sf
run sf2
