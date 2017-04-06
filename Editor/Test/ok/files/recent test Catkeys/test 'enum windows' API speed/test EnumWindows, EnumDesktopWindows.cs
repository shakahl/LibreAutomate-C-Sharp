/exe 1

 Problem: If no uiAccess, skips Metro windows. GetWindow too. IAccessible too. Only FindWindowEx doesn't skip, but it is almost 10 times slower than EnumWindows, which is almost 3 times slower than GetWindow.

out
PF
int n
EnumWindows &sub.Proc &n
 EnumDesktopWindows 0 &sub.Proc &n
 EnumChildWindows GetDesktopWindow &sub.Proc &n ;;gets all controls too (all descendants), but not Metro windows. Slower.
PN
PO
out n

#sub Proc
function# w int&n
n+1
 outw2 w
if GetWinStyle(w 1)&WS_EX_NOREDIRECTIONBITMAP
	outw2 w
ret 1

 BEGIN PROJECT
 main_function  test EnumWindows, EnumDesktopWindows
 exe_file  $my qm$\test EnumWindows, EnumDesktopWindows.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {07FBB467-D79E-4F78-80D0-3195E26F6A24}
 END PROJECT
