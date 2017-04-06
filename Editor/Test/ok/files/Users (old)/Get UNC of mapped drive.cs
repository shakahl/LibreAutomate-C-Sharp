 /exe 1
str localPath="Z:\MP3\Fleur\10-the string.mp3"
 str localPath="\\GINTARAS\F\MP3\Fleur\10-the string.mp3"
 out dir(localPath)
str s.all(500)
int n=500
int e=WNetGetUniversalName(localPath UNIVERSAL_NAME_INFO_LEVEL s &n)
if(e) out e; end _s.dllerror("" "" e)
lpstr unc; memcpy &unc s 4
out unc

 BEGIN PROJECT
 main_function  Macro763
 exe_file  $my qm$\Macro763.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {1F93047A-9AF1-4A99-9750-F634E3477FCD}
 END PROJECT
