 /exe
str s.expandpath("$my qm$\console.exe")
 RunConsole2 s
 out _spawnl(_P_WAIT s s "ttt" 0)
 run s "" "" "" 0x30400

 s.expandpath("$system$\ipconfig.exe")
 STARTUPINFOW si.cb=sizeof(si)
 PROCESS_INFORMATION pi
 if(!CreateProcessW(0 @F"''{s}'' /?" 0 0 0 0 0 0 &si &pi)) end _s.dllerror
 CloseHandle pi.hThread
 wait 0 H pi.hProcess
 CloseHandle pi.hProcess

int ec=_spawnl(0 _s.expandpath("$system$\ipconfig.exe") "ipconfig" "/?" 0); if(ec=-1) end _s.dllerror("" "C")
mes 1
 2

 BEGIN PROJECT
 main_function  Macro2326
 exe_file  $my qm$\Macro2326.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  70
 guid  {B70E9931-8169-4A83-97D8-B83ABCE9C897}
 END PROJECT
