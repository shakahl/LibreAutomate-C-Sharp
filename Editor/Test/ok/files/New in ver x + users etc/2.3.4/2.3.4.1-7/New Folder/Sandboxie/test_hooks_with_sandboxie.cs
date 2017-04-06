 /exe
 \
function nCode wParam MSLLHOOKSTRUCT*m
if(getopt(nargs)) goto gHook ;;called as hook procedure. Else runs on Alt+mouse left button trigger.
 ___________________________________________

int hhM=SetWindowsHookEx(WH_MOUSE_LL &test_hooks_with_sandboxie _hinst 0)
mes "test_hooks_with_sandboxie"
UnhookWindowsHookEx hhM
ret
 ___________________________________________

 gHook
ret CallNextHookEx(0 nCode wParam m)

 BEGIN PROJECT
 main_function  test_hooks_with_sandboxie
 exe_file  $my qm$\test_hooks_with_sandboxie.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 version  
 version_csv  
 flags  6
 end_hotkey  0
 guid  {8552C90C-517F-4BC2-9129-C4F1552DB716}
 END PROJECT
