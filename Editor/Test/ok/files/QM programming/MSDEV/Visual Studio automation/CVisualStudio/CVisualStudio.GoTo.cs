function $fileLine

 Opens fileLine in Visual Studio 9.
 If fileLine is like "file(line)", scrolls to the line.


opt noerrorshere 1
_Init
 act dte.MainWindow.HWnd ;;QM bug: if Excel functions used, thinks that MainWindow returns Excel.Window, and error because Excel.Window does not have HWnd
EnvDTE.Window w=dte.MainWindow; act w.HWnd ;;works
str s1 s2
tok fileLine &s1 2 "()"
dte.ExecuteCommand("File.OpenFile" s1)
if(s2.len) dte.ExecuteCommand("Edit.GoTo" s2)
