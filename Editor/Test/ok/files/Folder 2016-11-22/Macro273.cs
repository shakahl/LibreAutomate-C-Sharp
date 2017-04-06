 /exe

dll- shell32 #SHGetKnownFolderPath GUID*rfid dwFlags hToken @**ppszPath

out GetModuleHandle("shell32.dll")

PF
LoadLibrary("shell32.dll")
PN
word* w
if(SHGetKnownFolderPath(uuidof("{B4BFCC3A-DB2C-424C-B029-7FE99A87C641}") KF_FLAG_DONT_VERIFY 0 &w)) end "failed"
PN;PO
out "%S" w

 BEGIN PROJECT
 main_function  Macro273
 exe_file  $my qm$\Macro273.qmm
 flags  6
 guid  {C54F10F4-A967-4397-A7AC-3E5576BBFA6C}
 END PROJECT
