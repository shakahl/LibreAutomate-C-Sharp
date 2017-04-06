zip- "Q:\My QM\test\jab.zip " "Q:\My QM"

 str tempFile=F"$temp qm$\ver 0x{QMVER}\qmzip.dll"
 tempFile.expandpath
 out tempFile
 out FileExists(tempFile)

 BEGIN PROJECT
 main_function  Macro2566
 exe_file  $my qm$\Macro2566.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {EFC49099-26DF-4BD0-86A7-699A660F4C8A}
 END PROJECT
