 /exe

 qm_pe.exe source.
 _________________________________________

 creating exe?
function !creatingExe
if(creatingExe) ret

 qm installed?
if(rget(_s "" "Software\Microsoft\Windows\CurrentVersion\App Paths\qm.exe" HKEY_LOCAL_MACHINE)) mes- "Failed. QM is installed on this computer." "" "x"

 str sfpe rc
 sfpe.expandpath("$appdata$\gindi\quick macros\pe")

 uninstall?
int u=find(_command "/u" 0 1)>=0
if(u) goto UNINSTALL

 -------------------------------------------------------

 INSTALL

 import qm registry settings
int ire=!QP_ImportReg
if(!ire) if(!rget(_s "qmrc")) ire=-1
if(!ire) if(!rset(_s "QMX" "Software\Gindi")) ire=-2
if(ire) mes- "Failed to import QM settings. Error %i." "" "x" ire

 create some folders
mkdir "$appdata$\GinDi\Quick Macros"
mkdir "$common appdata$\GinDi\Quick Macros" ;;QM setup also would change permissions, but now not necessary

 run qm and wait
run "$qm$\app\qm.exe" "" "" "" 0x400

 -------------------------------------------------------

 UNINSTALL

 opt err 1

 delete registry
rset 0 "QMX" "Software\Gindi" 0 -1
rset 0 "qm2" "Software\Gindi" 0 -2
rset 0 "gindi" "Software" 0 -2

 delete files
del- "$appdata$\GinDi\Quick Macros"; if(!dir("$appdata$\GinDi\*" 2)) del- "$appdata$\GinDi"
del- "$common appdata$\GinDi\Quick Macros"; if(!dir("$common appdata$\GinDi\*" 2)) del- "$common appdata$\GinDi"

err+ mes "Failed.[]%s" "" "x" _error.description
