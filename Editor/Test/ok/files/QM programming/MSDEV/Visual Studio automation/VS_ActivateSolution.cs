 \
function $solutionPath

 Activates main window of specified Visual Studio 7.1 solution.

 solutionPath - solution path, like "Q:\app\app.sln".
   To display paths of currently open solutions, run macro VS_EnumROT.


#compile "__CVisualStudio"

EnvDTE.Solution sol._getactive(0 0 solutionPath)
act sol.DTE.MainWindow.HWnd
