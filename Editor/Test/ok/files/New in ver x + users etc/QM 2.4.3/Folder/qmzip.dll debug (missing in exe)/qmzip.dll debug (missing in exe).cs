if(0) zip- "c:\z.zip" "c:" ;;this just makes to add qmzip.dll to exe

str tempFile=F"$temp qm$\ver 0x{QMVER}\qmzip.dll"
int hm=ExeExtractFile(11 tempFile 0x101 252)
if(!hm) mes- F"ExeExtractFile failed.[]{GetLastError} {_s.dllerror}"
FreeLibrary hm

 BEGIN PROJECT
 main_function  Macro2567
 exe_file  $my qm$\Macro2567.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {026861A4-EB06-45C7-8F7D-EAFB94D16E93}
 END PROJECT
