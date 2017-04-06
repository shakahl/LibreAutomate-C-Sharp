 /exe 1

SetCurDir "c:\windows"

lpstr c=
 #include <windows.h>
 int add(int a, int b)
 {
 MessageBox(0, "info", "__Tcc in exe", MB_TOPMOST);
 return a+b;
 }

__Tcc x.Compile(c "add" 0 "user32")
out call(x.f 1 2)

 BEGIN PROJECT
 main_function  tcc in exe
 exe_file  $my qm$\Macro1282.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {CC00A7DC-2911-41FE-9AF0-420E9EC41C9E}
 END PROJECT
