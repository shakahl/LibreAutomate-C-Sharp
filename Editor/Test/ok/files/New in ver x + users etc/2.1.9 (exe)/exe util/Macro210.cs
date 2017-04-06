 mac+ "Macro214"
act id(2210 _hwndqm)
key Ct
1
 debug-run qm2.exe
int h=win("Visual C" "wndclass_desked_gsk")
act h
Acc a=acc("MyMacros.RecordingModule.TemporaryMacro" "MENUITEM" h "MsoCommandBar" "" 0x1081)
a.DoDefaultAction
