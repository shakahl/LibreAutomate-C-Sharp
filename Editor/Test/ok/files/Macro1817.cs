int hh=SetWinEventHook(EVENT_OBJECT_CREATE EVENT_OBJECT_CREATE 0 &sub.Hook_SetWinEventHook 0 0 WINEVENT_OUTOFCONTEXT)
if(!hh) end F"{ERR_FAILED}. {_s.dllerror}"
opt waitmsg 1
wait -1
UnhookWinEvent hh


#sub Hook_SetWinEventHook
function hHook event hwnd idObject idChild dwEventThread dwmsEventTime

 outw hwnd ;;debug
0.01

 your code here


  SetWinEventHook example

 BEGIN PROJECT
 main_function  Macro1817
 exe_file  $my qm$\WinEventHook.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {76EA3F09-A5C3-4F5C-AA63-9159412B9D7A}
 END PROJECT
