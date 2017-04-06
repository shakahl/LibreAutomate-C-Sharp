int hwnd=win("QM Reloading...")
if(hwnd) clo hwnd; ret ;;this macro runs after restarting, and closes the dialog

 create and run VBScript file that shows message box
str s=
 MsgBox "QM Reloading...", 0, "QM Reloading..."
str sf="$my qm$\vbs_qm_reloading.vbs"
s.setfile(sf)
run sf
act(wait(10 WV "QM Reloading..."))
 create command line to run this macro (assume, it is named RestartQM) when QM starts, and restart QM
s=
 M "RestartQM"
if(IsWindowVisible(_hwndqm)) s-"V "
shutdown -2 0 s
