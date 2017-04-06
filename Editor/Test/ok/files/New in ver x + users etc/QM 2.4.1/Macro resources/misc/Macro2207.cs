 /exe
 GUID g
 CoCreateGuid(&g)
 _s.fromn(&g sizeof(g))
 _s.encrypt(4 _s "" 3)
 out _s

 {96668C01-53AA-4C5B-AF22-AD939C277248}
 {bKT+kydKskeJioJP9D4mcQ}

 AddTrayIcon "resource:<Macro2207>blue.ico"
 AddTrayIcon "resource:<Macro2207>44788 shell32_dll_20.ico"
AddTrayIcon "resource:<{954807DF-2A82-434A-B6CD-18232AA154C5}Macro2207>44788 shell32_dll_20.ico"
3

 __Hicon hi
  hi=GetFileIcon("resource:<Macro2207>blue.ico")
 hi=GetFileIcon("resource:<Macro2207>44788 shell32_dll_20.ico")
 out hi

 BEGIN PROJECT
 main_function  Macro2207
 exe_file  $my qm$\Macro2207.qmm
 flags  6
 guid  {10C9EFF5-793C-469C-A800-31CB9CCE1041}
 END PROJECT
