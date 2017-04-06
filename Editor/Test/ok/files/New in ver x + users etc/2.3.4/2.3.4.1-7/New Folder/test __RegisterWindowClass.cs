/exe
out
__RegisterWindowClass g_wndClassTest
__Hicon hi=GetFileIcon("macro.ico")
__Hicon hi2=GetFileIcon("macro.ico" 0 1)
 g_wndClassTest.Register("QM_Test" &WndProc_Test)
 g_wndClassTest.Register("QM_Test" &WndProc_Test 0 F"{hi2} {hi}")
 g_wndClassTest.Register("QM_Test" &WndProc_Test 0 ":5 $qm$\macro.ico")
 g_wndClassTest.Register("QM_Test ąčę" &WndProc_Test 0 ":5 $qm$\macro.ico")
g_wndClassTest.Register("QM_Test ͜ϑף" &WndProc_Test 0 ":5 $qm$\macro.ico")
int w=CreateWindowExW(0 +g_wndClassTest.atom @"Test" WS_VISIBLE|WS_POPUP|WS_CAPTION|WS_SYSMENU 0 0 200 100 0 0 _hinst 0)
MessageLoop
 g_wndClassTest.Unregister

 BEGIN PROJECT
 main_function  Macro1793
 exe_file  $my qm$\Macro1793.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  22
 end_hotkey  0
 guid  {BB9E4EBF-67FB-4265-8D04-785268AF0364}
 END PROJECT
