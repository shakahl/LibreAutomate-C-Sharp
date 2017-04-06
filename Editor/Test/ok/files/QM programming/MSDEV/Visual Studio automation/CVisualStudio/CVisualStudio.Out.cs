function $s

 Displays s in Visual Studio 9 output window.
 If it is like "file(line)", you can double click to jump to the line. Or can be "file(line) : some text".


opt noerrorshere 1
_Init
EnvDTE.Window w=dte.Windows.Item(EnvDTE.vsWindowKindOutput)
w.Activate
EnvDTE.OutputWindowPane pane=w.Object.OutputWindowPanes.Item(1)
pane.Activate
pane.OutputString(_s.from(s "[]"))
