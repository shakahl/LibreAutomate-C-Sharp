WT_ResultsVisualClose
UnloadDll("$qm$\qmtc32.dll")
int w=win("dlg_apihook" "#32770")
if w
	 clo w
	key CASI ;;close the transparent results window; it ends DAH_RemoteCaptureWithDll which unloads dll
	0.05
