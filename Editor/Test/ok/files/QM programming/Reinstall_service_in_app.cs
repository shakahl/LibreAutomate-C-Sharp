 /exe 2
 Notes:
 QM must be running as admin.
 Process driver (qmphook) must not be loaded. Therefore we run this as exe and exit QM. Could instead temporarily uncheck Process in Options/Triggers, but may forget etc.

 run second instance of this process, because qm closes the first process when exits
_s=_command; if(_s!"x") run ExeFullPath "x"; ret

str sp="q:\app"
 str sp.expandpath("$program files$\quick macros 2") ;;use this instead if want to reinstall in PF, eg when testing a driver on 64-bit vmware PC
if(!dir(sp 2))
	sp="d:\myprojects\app" ;;notebook
	if(!dir(sp 2)) mes- "app folder not found"

men 101 win("" "QM_Editor"); err ;;Exit Program
wait 0 WP win("" "QM_Editor")
2

dll "$qm$\qmsetup.dll"
	ServiceInstall step @*destdir
	#ControlQmService action name @*destdir

 out ControlQmService(4 @"Quick Macros" "")
 ret

ServiceInstall 0 @""
2
ServiceInstall 1 @F"{sp}\"

run F"{sp}\qm.exe" "v"

 BEGIN PROJECT
 main_function  Reinstall_service_in_app
 exe_file  $my qm$\Reinstall service in app.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {B0981489-972E-4E2E-BCB2-184E0377A384}
 END PROJECT
