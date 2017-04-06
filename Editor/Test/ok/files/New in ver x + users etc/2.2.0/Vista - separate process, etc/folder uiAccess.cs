/exe 1
 out _winUAC

 int h=win("" "CabinetWClass")
 if(h) clo h; wait 2

 ShellExecute(0, 0, "explorer.exe", 0, 0, 0)
 ShellExecute(0, 0, _s.expandpath("$Program Files$"), 0, 0, 0)
 ShellExecute(0, 0, "explorer.exe", _s.expandpath("$Program Files$"), 0, 0)
 ShellExecute(0, 0, "explorer.exe", "c:\", 0, 0)

 run "explorer.exe"
 run "$desktop$\app (local).lnk"
 run "\\GINTARAS\app"
 run "$3$ 1E007180000000000000000000005427636023C5624BB45C4172DA012619"
 run "$desktop$\User Accounts - Shortcut.lnk"
 run "$desktop$\app.lnk"
 run "$desktop$\2BrightSparks - Shortcut.lnk"
run "$program files$"
 FolderExploreExpand "$Program Files$"
 run "$program files$" "" "" "" 0x20000
 run "explorer.exe" _s.expandpath("$program files$")
 run "no folder"
 run "::{21EC2020-3AEA-1069-A2DD-08002B30309D}\{E2E7934B-DCE5-43C4-9576-7FE4F75E7480}"
 run "::{21EC2020-3AEA-1069-A2DD-08002B30309D}"
 run "$desktop$\VistaSwitchUser.zip"
 10
 run "$3$" ;;Control Panel
 run "$3$ 1E007180000000000000000000008894CD1728122F4B88CE4298E93E0966" ;;Default Programs
 run ":: 14001F6880531C87A0426910A2EA08002B30309D" ;;Internet Explorer
 run "$system$\mshta.exe" "%SystemRoot%\system32\feedback.hta"
 run "$system$\mshta.exe" "c:\windows\system32\feedback.hta"

 BEGIN PROJECT
 main_function  folder uiAccess
 exe_file  $my qm$\Macro489.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {802EC1FC-5BE8-456A-9071-B780DFDA15E9}
 END PROJECT

 exe_file  $common appdata$\Macro489.exe
