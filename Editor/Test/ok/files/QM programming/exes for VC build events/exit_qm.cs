
int h=win("" "QM_Editor")
if h
	SendNotifyMessage(h, WM_CLOSE, 1, 0);
	wait 10 WP h
	0.1

 unload qmhook32.dll from background processes
str s=_command
if(find(s "/free_qmhook")>=0)
	int rm=RegisterWindowMessage("free_qmhook");
	SendMessageTimeout(HWND_BROADCAST, rm, 0, 0, SMTO_ABORTIFHUNG, 1000, &_i);
	PostMessage(HWND_BROADCAST, rm, 0, 0);

 note: don't use
 run "qmcl.exe" "/!"
 , because qmcl.exe may not exist.


 BEGIN PROJECT
 main_function  exit_qm
 exe_file  $qm$\app_plus\exit_qm.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {DE68025E-4407-45AE-9272-FEA34C5C1457}
 END PROJECT
