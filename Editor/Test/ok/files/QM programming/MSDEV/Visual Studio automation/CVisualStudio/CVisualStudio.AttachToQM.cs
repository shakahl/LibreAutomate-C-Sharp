

opt noerrorshere 1
_Init
EnvDTE.Window w=dte.MainWindow; act w.HWnd

 dte.ExecuteCommand("Debug.AttachToProcess" "")
EnvDTE.Process proc
foreach proc dte.Debugger.LocalProcesses
	_s=proc.Name
	OutputDebugString _s
	if _s.endi("qm.exe")
		proc.Attach
		break
