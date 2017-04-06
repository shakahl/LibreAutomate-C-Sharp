 /
function# $sfroot str&currentfile

str rc s3 s4 rks="Software\Gindi\QM2\settings"

 get regcode and file
if(!rget(rc "qmx" "Software\Gindi") and !rget(rc "qmx" "Software\Gindi" HKEY_LOCAL_MACHINE)) ret 101
if(!rset(rc "qmrc")) ret 101
rget currentfile "file" rks; currentfile.expandpath
if(!dir(currentfile)) ret 103

 export registry settings
RegExportXml _s.from(sfroot "\qmpe_reg.xml") "Software\GinDi\QM2"; err ret 102
