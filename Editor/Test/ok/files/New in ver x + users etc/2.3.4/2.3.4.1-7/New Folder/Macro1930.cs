 /exe

int atom=RegWinClass("test" &DefWindowProc 0 _dialogicon)
int h=CreateWindowEx(0 +atom "ttt" WS_OVERLAPPEDWINDOW|WS_VISIBLE 0 0 200 100 0 0 _hinst 0)
opt waitmsg 1
wait 5
DestroyWindow h

 BEGIN PROJECT
 main_function  Macro1930
 exe_file  $my qm$\Macro1930.qmm
 icon  <default>
 flags  6
 guid  {0BFEF359-3ABF-4A4F-BC17-18727E4F9E15}
 END PROJECT
